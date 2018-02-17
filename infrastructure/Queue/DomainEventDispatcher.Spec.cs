﻿using Autofac;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using FluentAssertions;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.Common.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using MidnightLizard.Schemes.Infrastructure.AutofacModules;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common;
using MidnightLizard.Schemes.Tests;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TransEvent = MidnightLizard.Schemes.Domain.Common.Messaging.TransportMessage<MidnightLizard.Schemes.Domain.Common.Messaging.DomainEvent<MidnightLizard.Schemes.Domain.PublicSchemeAggregate.PublicSchemeId>, MidnightLizard.Schemes.Domain.PublicSchemeAggregate.PublicSchemeId>;

namespace MidnightLizard.Schemes.Infrastructure.Queue
{
    public class DomainEventDispatcherSpec : DomainEventDispatcher<PublicSchemeId>
    {
        private readonly string testMessageJson = "{Type:\"Test\"}";
        private int GetEventTopicName_CallCount = 0;
        private readonly TransEvent testTransEvent = new TransEvent(
            new SchemePublishedEvent(
                new PublicSchemeId(Guid.NewGuid()),
                new PublisherId(Guid.NewGuid()),
                ColorSchemeSpec.CorrectColorScheme),
            Guid.NewGuid(), DateTime.UtcNow);

        public DomainEventDispatcherSpec() : base(
            new KafkaConfig
            {
                KAFKA_EVENTS_PRODUCER_CONFIG = new Dictionary<string, object>
                {
                    ["bootstrap.servers"] = "bootstrap.kafka:9092"
                }
            },
            Substitute.For<IMessageSerializer>())
        {
            this.producer = Substitute.For<ISerializingProducer<string, string>>();
            this.producer.ProduceAsync(
                this.GetEventTopicName(),
                this.testTransEvent.Payload.AggregateId.ToString(),
                Arg.Any<string>())
                .Returns(new Message<string, string>("", 0, 0, "", "", new Timestamp(), new Error(ErrorCode.NoError)));
            this.messageSerializer.Serialize(this.testTransEvent).Returns(this.testMessageJson);
        }

        //private readonly IDomainEventDispatcher<PublicSchemeId> eventDispatcher;

        //public DomainEventDispatcherSpec()
        //{
        //var builder = new ContainerBuilder();
        //builder.RegisterModule<DomainInfrastructureModule>();
        //builder.RegisterInstance<KafkaConfig>(new KafkaConfig());
        //var container = builder.Build();
        //eventDispatcher = container.Resolve<IDomainEventDispatcher<PublicSchemeId>>();
        //}

        protected override string GetEventTopicName()
        {
            this.GetEventTopicName_CallCount++;
            return "Test";
        }

        public class DispatchEventSpec : DomainEventDispatcherSpec
        {
            [It(nameof(DispatchEvent))]
            public async Task Should_serialize_TransportEvent()
            {
                var result = await this.DispatchEvent(testTransEvent);

                this.messageSerializer.Received(1).Serialize(testTransEvent);
            }

            [It(nameof(DispatchEvent))]
            public async Task Should_call_GetEventTopicName()
            {
                this.GetEventTopicName_CallCount = 0;

                var result = await this.DispatchEvent(testTransEvent);

                this.GetEventTopicName_CallCount.Should().Be(1);
            }

            [It(nameof(DispatchEvent))]
            public async Task Should_Produce_KafkaMessage()
            {
                var result = await this.DispatchEvent(testTransEvent);

                await this.producer.Received(1).ProduceAsync(
                    this.GetEventTopicName(),
                    this.testTransEvent.Payload.AggregateId.ToString(),
                    this.testMessageJson);
            }

            [It(nameof(DispatchEvent))]
            public async Task Should_return_Error_when_Exception_thrown()
            {
                this.messageSerializer.Serialize(this.testTransEvent)
                    .Returns(x => throw new Exception());

                var result = await this.DispatchEvent(testTransEvent);

                result.HasError.Should().BeTrue();
                result.Exception.Should().NotBeNull();
            }

            [It(nameof(DispatchEvent))]
            public async Task Should_return_Error_when_Producer_does()
            {
                this.producer.ProduceAsync(
                    this.GetEventTopicName(),
                    this.testTransEvent.Payload.AggregateId.ToString(),
                    this.testMessageJson).Returns(new Message<string, string>("", 0, 0, "", "", new Timestamp(),
                        new Error(ErrorCode.Unknown)));

                var result = await this.DispatchEvent(testTransEvent);

                result.HasError.Should().BeTrue();
            }
        }
    }
}