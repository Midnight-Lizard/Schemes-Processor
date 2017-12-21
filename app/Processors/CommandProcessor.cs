using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;

namespace MidnightLizard.Schemes.Processor.Processors
{
    public interface ICommandProcessor : IDisposable
    {
        void Start();
    }

    public class CommandProcessor : ICommandProcessor
    {
        protected readonly ILogger<CommandProcessor> logger;
        protected readonly Consumer<string, string> consumer;

        public CommandProcessor(ILogger<CommandProcessor> logger)
        {
            this.logger = logger;
            this.logger.LogInformation("CommandProcessor ctor");
            this.consumer = new Consumer<string, string>(
                new Dictionary<string, object>() {
                    { "group.id", "command-processor" },
                    { "bootstrap.servers", "bootstrap.kafka:9092" },
                    { "enable.auto.commit", false }
                },
                new StringDeserializer(Encoding.UTF8),
                new StringDeserializer(Encoding.UTF8));
        }

        public void Start()
        {
            var lastOffset = 0;
            this.logger.LogInformation("Starting CommandProcessor");
            this.consumer.Assign(new[]
            {
                new TopicPartitionOffset("test", 0, lastOffset)
            });
            // this.consumer.Subscribe(new[] { "test" });
            this.consumer.OnMessage += ConsumerOnMessage;
            this.consumer.OnError += ConsumerOnError;
            while (true)
            {
                this.consumer.Poll(1000);
            }
        }

        private void ConsumerOnError(object sender, Error e)
        {
            this.logger.LogError("ConsumerOnError: " + e.Reason);
        }

        private void ConsumerOnMessage(object sender, Message<string, string> msg)
        {
            this.logger.LogInformation($"[{msg.Key}]=[{msg.Value}]");
            // this.consumer.CommitAsync(msg);
        }

        public void Dispose()
        {
            this.consumer.Dispose();
        }
    }
}