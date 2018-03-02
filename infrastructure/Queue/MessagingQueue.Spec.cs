using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using MidnightLizard.Schemes.Infrastructure.Configuration;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common;
using MidnightLizard.Schemes.Infrastructure.Versioning;
using MidnightLizard.Testing.Utilities;
using NSubstitute;

using TransEvent = MidnightLizard.Commons.Domain.Messaging.TransportMessage<MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events.SchemePublishedEvent, MidnightLizard.Schemes.Domain.PublicSchemeAggregate.PublicSchemeId>;

namespace MidnightLizard.Schemes.Infrastructure.Queue
{
    public class MessagingQueueSpec : MessagingQueue
    {
        private readonly TransEvent correctTransEvent = new TransEvent(
                new SchemePublishedEvent(
                    new PublicSchemeId(Guid.NewGuid()),
                    new PublisherId(Guid.NewGuid()),
                    ColorSchemeSpec.CorrectColorScheme),
                Guid.NewGuid(), DateTime.UtcNow);
        private readonly string correctMessageJson;
        private readonly Message<string, string> correctKafkaMessage;

        public MessagingQueueSpec() : base(
            Substitute.For<ILogger<MessagingQueue>>(),
            Substitute.For<KafkaConfig>(),
            Substitute.For<IMediator>(),
            Substitute.For<IMessageSerializer>())
        {
            this.correctMessageJson = new MessageSerializer(Latest.Version, null).SerializeMessage(this.correctTransEvent);
            this.correctKafkaMessage = this.CreateKafkaMessage(this.correctMessageJson, ErrorCode.NoError);

            this.messageSerializer.Deserialize(this.correctKafkaMessage.Value, this.correctKafkaMessage.Timestamp.UtcDateTime)
                  .Returns(new MessageResult(this.correctTransEvent));
            this.mediator.Send(this.correctTransEvent)
                .Returns(DomainResult.Ok);
        }

        private Message<string, string> CreateKafkaMessage(string json, ErrorCode errorCode)
        {
            return new Message<string, string>("", 0, 0, "", json,
                new Timestamp(DateTime.UtcNow, TimestampType.CreateTime),
                new Error(errorCode, "Who cares"));
        }

        public class HandleMessageSpec : MessagingQueueSpec
        {
            [It(nameof(HandleMessage))]
            public async Task Should_send_Message_to_Mediator()
            {
                await this.HandleMessage(this.correctKafkaMessage);

                await this.mediator.Received(1).Send(this.correctTransEvent);
            }

            [It(nameof(HandleMessage))]
            public async Task Should_Deserialize_KafkaMessage__Value()
            {
                await this.HandleMessage(this.correctKafkaMessage);

                this.messageSerializer.Received(1)
                    .Deserialize(this.correctKafkaMessage.Value, this.correctKafkaMessage.Timestamp.UtcDateTime);
            }

            [It(nameof(HandleMessage))]
            public async Task Should_log_Successful_results_on_HappyPath()
            {
                await this.HandleMessage(this.correctKafkaMessage);

                this.logger.Received(1).Log(LogLevel.Information, 0, Arg.Any<FormattedLogValues>(), null, Arg.Any<Func<object, Exception, string>>());
            }

            [It(nameof(HandleMessage))]
            public async Task Should_log_Error_when_KafkaMessage_HasError()
            {
                var errorKafkaMessage = this.CreateKafkaMessage(this.correctMessageJson, ErrorCode.BrokerNotAvailable);

                await this.HandleMessage(errorKafkaMessage);

                this.logger.Received(1).Log(LogLevel.Error, 0, Arg.Any<FormattedLogValues>(), null, Arg.Any<Func<object, Exception, string>>());
            }

            [It(nameof(HandleMessage))]
            public async Task Should_not_Deserialize_KafkaMessage_when_it_HasError()
            {
                var errorKafkaMessage = this.CreateKafkaMessage(this.correctMessageJson, ErrorCode.BrokerNotAvailable);

                await this.HandleMessage(errorKafkaMessage);

                this.messageSerializer.DidNotReceiveWithAnyArgs().Deserialize(default, default);
            }

            [It(nameof(HandleMessage))]
            public async Task Should_not_Send_Message_to_Mediator_when_Deserialization_failed()
            {
                this.messageSerializer.Deserialize(this.correctKafkaMessage.Value, this.correctKafkaMessage.Timestamp.UtcDateTime)
                   .Returns(new MessageResult("error"));

                await this.HandleMessage(this.correctKafkaMessage);

                await this.mediator.DidNotReceiveWithAnyArgs().Send(default);
            }

            [It(nameof(HandleMessage))]
            public async Task Should_log_Error_when_Deserialization_failed()
            {
                this.messageSerializer.Deserialize(this.correctKafkaMessage.Value, this.correctKafkaMessage.Timestamp.UtcDateTime)
                   .Returns(new MessageResult("error"));

                await this.HandleMessage(this.correctKafkaMessage);

                this.logger.Received(1).Log(LogLevel.Error, 0, Arg.Any<FormattedLogValues>(), null, Arg.Any<Func<object, Exception, string>>());
            }
        }
    }
}
