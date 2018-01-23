using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;
using MidnightLizard.Schemes.Infrastructure.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Processor.Processors
{
    public interface ISchemesProcessor : IDisposable
    {
        void Pause();
        Task Start(CancellationToken token);
    }

    public class SchemesProcessor : ISchemesProcessor
    {
        protected bool isRunning = false;
        protected bool assignedRequestsPartitionsArePaused = false;
        protected Message<string, string> lastConsumedMessage;
        protected readonly TimeSpan timeout = TimeSpan.FromSeconds(1);
        protected readonly HashSet<TopicPartition> assignedEventsPartitions = new HashSet<TopicPartition>();
        protected readonly HashSet<TopicPartition> assignedRequestsPartitions = new HashSet<TopicPartition>();
        protected readonly ILogger<SchemesProcessor> logger;
        private readonly KafkaConfig kafkaConfig;
        protected readonly Consumer<string, string> consumer;
        protected CancellationToken cancellationToken;

        public SchemesProcessor(ILogger<SchemesProcessor> logger, KafkaConfig config)
        {
            this.logger = logger;
            this.kafkaConfig = config;
            this.consumer = new Consumer<string, string>(
                config.KAFKA_CONSUMER_CONFIG,
                new StringDeserializer(Encoding.UTF8),
                new StringDeserializer(Encoding.UTF8));
            this.consumer.OnOffsetsCommitted += ConsumerOnOffsetsCommitted;
            this.consumer.OnPartitionsAssigned += ConsumerOnPartitionsAssigned;
            this.consumer.OnPartitionsRevoked += ConsumerOnPartitionsRevoked;
            this.consumer.OnMessage += ConsumerOnMessage;
            this.consumer.OnError += ConsumerOnError;
        }

        public async Task Start(CancellationToken token)
        {
            this.cancellationToken = token;
            this.cancellationToken.Register(this.Dispose);
            this.isRunning = true;
            this.consumer.Subscribe(new[] {
                 kafkaConfig.SCHEMES_EVENTS_TOPIC,
                 kafkaConfig.SCHEMES_REQUESTS_TOPIC });
            while (this.isRunning && !this.cancellationToken.IsCancellationRequested)
            {
                ValidateTopicPriority();
                if (!this.cancellationToken.IsCancellationRequested)
                {
                    this.consumer.Poll(timeout);
                }
                if (this.lastConsumedMessage != null)
                {
                    await this.consumer.CommitAsync(this.lastConsumedMessage);
                    this.lastConsumedMessage = null;
                }
            }
            this.isRunning = false;
        }

        private void ValidateTopicPriority()
        {
            if (assignedEventsPartitions.Count > 0)
            {
                bool hasNewEvents = false;
                try
                {
                    var currentEventPositions = this.consumer.Committed(assignedEventsPartitions, timeout);
                    if (!currentEventPositions.Exists(pos => pos.Error.HasError))
                    {
                        foreach (var curPos in currentEventPositions)
                        {
                            try
                            {
                                var finPos = this.consumer.QueryWatermarkOffsets(curPos.TopicPartition, timeout);
                                if (finPos.High != 0 && (curPos.Offset < finPos.High || finPos.High == Offset.Invalid))
                                {
                                    hasNewEvents = true;
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                this.logger.LogError(ex, "Failed to obtain watermark offsets");
                                hasNewEvents = true; // since I'm not sure
                                break;
                            }
                        }
                    }
                    else
                    {
                        hasNewEvents = true; // since I'm not sure
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Failed to obtain commited offsets for events topic");
                    hasNewEvents = true; // since I'm not sure
                }
                if (hasNewEvents && !this.assignedRequestsPartitionsArePaused)
                {
                    this.consumer.Pause(this.assignedRequestsPartitions);
                    this.assignedRequestsPartitionsArePaused = true;
                }
                else if (!hasNewEvents && this.assignedRequestsPartitionsArePaused)
                {
                    this.assignedRequestsPartitionsArePaused = false;
                    this.consumer.Resume(this.assignedRequestsPartitions);
                }
            }
        }

        private void ConsumerOnPartitionsRevoked(object sender, List<TopicPartition> partitions)
        {
            this.consumer.Unassign();
            assignedEventsPartitions.Clear();
            assignedRequestsPartitions.Clear();
        }

        private void ConsumerOnPartitionsAssigned(object sender, List<TopicPartition> partitions)
        {
            if (!this.cancellationToken.IsCancellationRequested)
            {
                this.consumer.Assign(partitions);
                partitions.ForEach(p =>
                {
                    if (p.Topic == kafkaConfig.SCHEMES_EVENTS_TOPIC)
                    {
                        assignedEventsPartitions.Add(p);
                    }
                    else
                    {
                        assignedRequestsPartitions.Add(p);
                    }
                });
                ValidateTopicPriority();
            }
        }

        private void ConsumerOnError(object sender, Error e)
        {
            this.logger.LogError($"ConsumerOnError: {e}");
        }

        private void ConsumerOnMessage(object sender, Message<string, string> msg)
        {
            this.logger.LogInformation($"ConsumerOnMessage: [{msg.Key}]=[{msg.Value}]");
            this.lastConsumedMessage = msg;
        }

        private void ConsumerOnOffsetsCommitted(object sender, CommittedOffsets offsets)
        {
            this.logger.LogInformation("ConsumerOnOffsetsCommitted: " + string.Join("\n", offsets.Offsets.Select(o => o.ToString())));
        }

        public async void Dispose()
        {
            while (this.isRunning)
            {
                await Task.Delay(timeout);
            };
            this.consumer.Dispose();
        }

        public void Pause()
        {
            this.isRunning = false;
        }
    }
}