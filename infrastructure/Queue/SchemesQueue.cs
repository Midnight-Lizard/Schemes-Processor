using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
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

    public class SchemesQueue
    {
        protected QueueStatus queueStatus = QueueStatus.Stopped;
        protected Message<string, string> lastConsumedEvent;
        protected Message<string, string> lastConsumedRequest;
        protected readonly TimeSpan timeout = TimeSpan.FromSeconds(1);
        protected List<TopicPartition> assignedEventsPartitions;
        protected readonly ILogger<SchemesQueue> logger;
        private readonly KafkaConfig kafkaConfig;
        protected Consumer<string, string> eventsConsumer;
        protected Consumer<string, string> requestsConsumer;
        protected CancellationToken cancellationToken;
        protected TaskCompletionSource<bool> queuePausingCompleted;

        public SchemesQueue(ILogger<SchemesQueue> logger, KafkaConfig config)
        {
            this.logger = logger;
            this.kafkaConfig = config;
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

                        this.eventsConsumer.OnOffsetsCommitted += ConsumerOnOffsetsCommitted;
                        this.eventsConsumer.OnPartitionsAssigned += EventsConsumerOnPartitionsAssigned;
                        this.eventsConsumer.OnPartitionsRevoked += EventsConsumerOnPartitionsRevoked;
                        this.eventsConsumer.OnMessage += EventsConsumerOnMessage;
                        this.eventsConsumer.OnError += ConsumerOnError;

                        this.requestsConsumer.OnOffsetsCommitted += ConsumerOnOffsetsCommitted;
                        // this.requestsConsumer.OnPartitionsAssigned += RequestsConsumerOnPartitionsAssigned;
                        // this.requestsConsumer.OnPartitionsRevoked += RequestsConsumerOnPartitionsRevoked;
                        this.requestsConsumer.OnMessage += RequestsConsumerOnMessage;
                        this.requestsConsumer.OnError += ConsumerOnError;

                        this.eventsConsumer.Subscribe(kafkaConfig.SCHEMES_EVENTS_TOPIC);
                        this.requestsConsumer.Subscribe(kafkaConfig.SCHEMES_REQUESTS_TOPIC);

                        while (this.queueStatus == QueueStatus.Running && !this.cancellationToken.IsCancellationRequested)
                        {
                            if (HasNewMessages(this.eventsConsumer, this.assignedEventsPartitions))
                            {
                                this.eventsConsumer.Poll(timeout);
                                if (this.lastConsumedEvent != null)
                                {
                                    await this.eventsConsumer.CommitAsync(this.lastConsumedEvent);
                                    this.lastConsumedEvent = null;
                                }
                            }
                            else
                            {
                                this.requestsConsumer.Poll(timeout);
                                if (this.lastConsumedRequest != null)
                                {
                                    await this.requestsConsumer.CommitAsync(this.lastConsumedRequest);
                                    this.lastConsumedRequest = null;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Failed to poll consumers");
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

        private bool HasNewMessages(Consumer<string, string> consumer, List<TopicPartition> partitions)
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

        private void EventsConsumerOnPartitionsRevoked(object sender, List<TopicPartition> partitions)
        {
            this.eventsConsumer.Unassign();
            assignedEventsPartitions = null;
        }

        private void RequestsConsumerOnPartitionsRevoked(object sender, List<TopicPartition> partitions)
        {
            this.requestsConsumer.Unassign();
        }

        private void EventsConsumerOnPartitionsAssigned(object sender, List<TopicPartition> partitions)
        {
            if (!this.cancellationToken.IsCancellationRequested)
            {
                this.eventsConsumer.Assign(partitions);
                assignedEventsPartitions = partitions;
            }
        }

        private void RequestsConsumerOnPartitionsAssigned(object sender, List<TopicPartition> partitions)
        {
            if (!this.cancellationToken.IsCancellationRequested)
            {
                this.requestsConsumer.Assign(partitions);
            }
        }

        private void ConsumerOnError(object sender, Error e)
        {
            this.logger.LogError($"ConsumerOnError: {e}");
        }

        private void EventsConsumerOnMessage(object sender, Message<string, string> msg)
        {
            this.logger.LogInformation($"ConsumerOnMessage: [{msg.Key}]=[{msg.Value}]");
            this.lastConsumedEvent = msg;
        }

        private void RequestsConsumerOnMessage(object sender, Message<string, string> msg)
        {
            this.logger.LogInformation($"ConsumerOnMessage: [{msg.Key}]=[{msg.Value}]");
            this.lastConsumedRequest = msg;
        }

        private void ConsumerOnOffsetsCommitted(object sender, CommittedOffsets offsets)
        {
            this.logger.LogInformation("ConsumerOnOffsetsCommitted: " + string.Join("\n", offsets.Offsets.Select(o => o.ToString())));
        }

        public async Task Pause()
        {
            this.queuePausingCompleted = new TaskCompletionSource<bool>();
            this.queueStatus = QueueStatus.Paused;
            await this.queuePausingCompleted.Task;
        }

        public async Task Resume(CancellationToken token)
        {
            this.queueStatus = QueueStatus.Stopped;
            await BeginProcessing(token);
        }
    }
}
