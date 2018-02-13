using MidnightLizard.Schemes.Tests;
using System;
using System.Collections.Generic;
using FluentAssertions;
using System.Text;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using Autofac;
using MidnightLizard.Schemes.Infrastructure.AutofacModules;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using Newtonsoft.Json.Linq;

using TransEvent = MidnightLizard.Schemes.Domain.Common.Messaging.TransportMessage<MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events.SchemePublishedEvent, MidnightLizard.Schemes.Domain.PublicSchemeAggregate.PublicSchemeId>;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common.Converters;
using Newtonsoft.Json;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Common
{
    public class MessageSerializerSpec
    {
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = MessageContractResolver.Default,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Converters = new[] {
                    new DomainEntityIdConverter()
                }
        };
        private readonly IMessageSerializer messageSerializer;
        public MessageSerializerSpec()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<MessageSerializationModule>();
            var container = builder.Build();
            messageSerializer = container.Resolve<IMessageSerializer>();
        }

        public class SerializeSpec : MessageSerializerSpec
        {
            private readonly TransEvent testTransEvent = new TransEvent(
                    new SchemePublishedEvent(new PublicSchemeId(), new PublisherId(), ColorSchemeSpec.CorrectColorScheme),
                    Guid.NewGuid(), DateTime.Now);

            [It(nameof(MessageSerializer.Serialize))]
            public void Should_correctly_serialize_event()
            {
                var json = this.messageSerializer.Serialize(this.testTransEvent);
                var obj = JObject.Parse(json);

                obj[nameof(Type)].Value<string>().Should().Be(nameof(SchemePublishedEvent));
                obj[nameof(Version)].Value<string>().Should().Be(this.testTransEvent.Payload.LatestVersion().ToString());
                obj[nameof(TransEvent.RequestTimestamp)].Value<DateTime>().Should().Be(this.testTransEvent.RequestTimestamp);
                obj[nameof(TransEvent.CorrelationId)].ToObject<Guid>().Should().Be(this.testTransEvent.CorrelationId);
                var @event = obj[nameof(TransEvent.Payload)].ToObject<SchemePublishedEvent>(JsonSerializer.Create(serializerSettings));
                @event.Id.Should().Be(this.testTransEvent.Payload.Id);
                @event.AggregateId.Should().Be(this.testTransEvent.Payload.AggregateId);
                @event.Generation.Should().Be(this.testTransEvent.Payload.Generation);
                @event.PublisherId.Should().Be(this.testTransEvent.Payload.PublisherId);
                @event.ColorScheme.Should().Be(this.testTransEvent.Payload.ColorScheme);
            }
        }

        public class DeserializeSpec : MessageSerializerSpec
        {

        }
    }
}
