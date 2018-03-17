﻿using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Schemes.Infrastructure.Configuration;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Queue
{
    public enum QueueStatus
    {
        Stopped = 0,
        Running = 1,
        Paused = 2
    }

    public class MessagingQueue : IMessagingQueue
    {
        protected QueueStatus queueStatus = QueueStatus.Stopped;
        protected Message<string, string> lastConsumedEvent;
        protected Message<string, string> lastConsumedRequest;
        protected readonly TimeSpan timeout = TimeSpan.FromSeconds(1);
        protected List<TopicPartition> assignedEventsPartitions;
        protected readonly ILogger<MessagingQueue> logger;
        protected readonly KafkaConfig kafkaConfig;
        protected readonly IMediator mediator;
        protected readonly IMessageSerializer messageSerializer;
        protected Consumer<string, string> eventsConsumer;
        protected Consumer<string, string> requestsConsumer;
        protected CancellationToken cancellationToken;
        protected TaskCompletionSource<bool> queuePausingCompleted;
        private bool thereMayBeNewMessages;

        public MessagingQueue(ILogger<MessagingQueue> logger, KafkaConfig config,
            IMediator mediator, IMessageSerializer messageSerializer)
        {
            this.logger = logger;
            this.kafkaConfig = config;
            this.mediator = mediator;
            this.messageSerializer = messageSerializer;
        }

        public async Task PauseProcessing()
        {
            this.queuePausingCompleted = new TaskCompletionSource<bool>();
            this.queueStatus = QueueStatus.Paused;
            await this.queuePausingCompleted.Task;
        }

        public async Task ResumeProcessing(CancellationToken token)
        {
            this.queueStatus = QueueStatus.Stopped;
            await BeginProcessing(token);
        }

        public async Task BeginProcessing(CancellationToken token)
        {
            if (this.cancellationToken != token)
            {
                this.cancellationToken = token;
                token.Register(async () =>
                {
                    while (this.queueStatus == QueueStatus.Running)
                    {
                        await Task.Delay(timeout);
                    }
                });
            }

            if (this.queueStatus == QueueStatus.Stopped)
            {
                try
                {
                    this.queueStatus = QueueStatus.Running;
                    using (Consumer<string, string>
                        eventsConsumer = new Consumer<string, string>(
                            this.kafkaConfig.KAFKA_EVENTS_CONSUMER_CONFIG,
                            new StringDeserializer(Encoding.UTF8),
                            new StringDeserializer(Encoding.UTF8)),
                        requestsConsumer = new Consumer<string, string>(
                            this.kafkaConfig.KAFKA_REQUESTS_CONSUMER_CONFIG,
                            new StringDeserializer(Encoding.UTF8),
                            new StringDeserializer(Encoding.UTF8)))
                    {
                        this.eventsConsumer = eventsConsumer;
                        this.requestsConsumer = requestsConsumer;

                        this.eventsConsumer.OnPartitionsAssigned += EventsConsumerOnPartitionsAssigned;
                        this.eventsConsumer.OnPartitionsRevoked += EventsConsumerOnPartitionsRevoked;

                        this.eventsConsumer.Subscribe(kafkaConfig.EVENT_TOPICS);
                        this.requestsConsumer.Subscribe(kafkaConfig.REQUEST_TOPICS);

                        while (this.queueStatus == QueueStatus.Running && !this.cancellationToken.IsCancellationRequested)
                        {
                            if (this.thereMayBeNewMessages && HasNewMessages(this.eventsConsumer, this.assignedEventsPartitions))
                            {
                                if (this.eventsConsumer.Consume(out var @event, timeout))
                                {
                                    await this.HandleMessage(@event);
                                    await this.eventsConsumer.CommitAsync(@event);
                                }
                            }
                            else
                            {
                                this.thereMayBeNewMessages = false;
                                if (this.requestsConsumer.Consume(out var request, timeout))
                                {
                                    this.thereMayBeNewMessages = true;
                                    await this.HandleMessage(request);
                                    await this.requestsConsumer.CommitAsync(request);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Failed to consume new messages");
                }
                finally
                {
                    switch (this.queueStatus)
                    {
                        case QueueStatus.Paused:
                            this.queuePausingCompleted?.SetResult(true);
                            break;
                        case QueueStatus.Running:
                        case QueueStatus.Stopped:
                        default:
                            this.queueStatus = QueueStatus.Stopped;
                            break;
                    }
                }
            }
        }

        protected bool HasNewMessages(Consumer<string, string> consumer, List<TopicPartition> partitions)
        {
            if (partitions == null || partitions.Count == 0) return true;
            try
            {
                var currentEventPositions = consumer.Committed(partitions, timeout);
                if (!currentEventPositions.Exists(pos => pos.Error.HasError))
                {
                    foreach (var curPos in currentEventPositions)
                    {
                        try
                        {
                            var finPos = consumer.QueryWatermarkOffsets(curPos.TopicPartition, timeout);
                            if (finPos.High != 0 && (curPos.Offset < finPos.High || finPos.High == Offset.Invalid))
                            {
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            this.logger.LogError(ex, "Failed to obtain watermark offsets");
                            return true; // since I'm not sure
                        }
                    }
                }
                else
                {
                    return true; // since I'm not sure
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to obtain commited offsets");
                return true; // since I'm not sure
            }
            return false;
        }

        protected void EventsConsumerOnPartitionsRevoked(object sender, List<TopicPartition> partitions)
        {
            this.eventsConsumer.Unassign();
            assignedEventsPartitions = null;
        }

        protected void EventsConsumerOnPartitionsAssigned(object sender, List<TopicPartition> partitions)
        {
            if (!this.cancellationToken.IsCancellationRequested)
            {
                this.eventsConsumer.Assign(partitions);
                assignedEventsPartitions = partitions;
            }
        }

        protected async Task HandleMessage(Message<string, string> kafkaMessage)
        {
            if (kafkaMessage.Error.HasError)
            {
                this.logger.LogError($"Failed to consume [{kafkaMessage.Value ?? "message"}] with reason: {kafkaMessage.Error.Reason}");
            }
            else
            {
                var deserializationResult = this.messageSerializer.Deserialize(kafkaMessage.Value, kafkaMessage.Timestamp.UtcDateTime);
                if (!deserializationResult.HasError)
                {
                    var message = deserializationResult.Message;
                    var info = JsonConvert.SerializeObject(new
                    {
                        message.CorrelationId,
                        message.Payload.Id,
                        Type = message.Payload.GetType().Name
                    });
                    var handleResult = await this.mediator.Send(message);
                    if (handleResult.HasError)
                    {
                        if (handleResult.Exception != null)
                        {
                            this.logger.LogError(handleResult.Exception, $"Failed to handle {info}");
                        }
                        else
                        {
                            this.logger.LogError($"Failed to handle {info} with error: {handleResult.ErrorMessage}");
                        }
                    }
                    else
                    {
                        this.logger.LogInformation(info);
                    }
                }
                else
                {
                    if (deserializationResult.Exception != null)
                    {
                        this.logger.LogError(deserializationResult.Exception, $"Failed to deserialize: [{kafkaMessage.Value}]");
                    }
                    else
                    {
                        this.logger.LogError($"Failed to deserialize [{kafkaMessage.Value}] with error: {deserializationResult.ErrorMessage}");
                    }
                }
            }
        }
    }
}