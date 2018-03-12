using MidnightLizard.Testing.Utilities;
using System;
using System.Collections.Generic;
using FluentAssertions;
using System.Text;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using Autofac;
using MidnightLizard.Schemes.Infrastructure.AutofacModules;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

using Event = MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events.SchemePublishedEvent;
using TransEvent = MidnightLizard.Commons.Domain.Messaging.TransportMessage<MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events.SchemePublishedEvent, MidnightLizard.Schemes.Domain.PublicSchemeAggregate.PublicSchemeId>;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common.Converters;
using MidnightLizard.Schemes.Infrastructure.Versioning;
using MidnightLizard.Commons.Domain.Model;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Common
{
    public class MessageSerializerSpec
    {
        private readonly IMessageSerializer messageSerializer;
        private readonly TransEvent testTransEvent = new TransEvent(
                new SchemePublishedEvent(
                    new PublicSchemeId(Guid.NewGuid()),
                    new PublisherId("test-user-id"),
                    ColorSchemeSpec.CorrectColorScheme),
                Guid.NewGuid(), DateTime.UtcNow, new UserId("test-user-id"));

        public MessageSerializerSpec()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<MessageSerializationModule>();
            builder.RegisterInstance(AppVersion.Latest);
            var container = builder.Build();
            messageSerializer = container.Resolve<IMessageSerializer>();
        }

        public class SerializeSpec : MessageSerializerSpec
        {
            [It(nameof(MessageSerializer.SerializeMessage))]
            public void Should_correctly_Serialize_event()
            {
                var json = this.messageSerializer.SerializeMessage(this.testTransEvent);
                var obj = JObject.Parse(json);

                obj[nameof(TransEvent.CorrelationId)].ToObject<Guid>().Should().Be(this.testTransEvent.CorrelationId);
                obj[nameof(Type)].Value<string>().Should().Be(nameof(SchemePublishedEvent));
                obj[nameof(Version)].Value<string>().Should().Be(AppVersion.Latest.ToString());
                obj[nameof(UserId)].Value<string>().Should().Be(this.testTransEvent.UserId.Value);
                obj[nameof(TransEvent.RequestTimestamp)].Value<DateTime>().Should().Be(this.testTransEvent.RequestTimestamp);

                var payload = obj[nameof(TransEvent.Payload)];

                payload[nameof(Event.Id)].ToObject<Guid>().Should().Be(this.testTransEvent.Payload.Id);
                payload[nameof(Event.AggregateId)].ToObject<Guid>().Should().Be(this.testTransEvent.Payload.AggregateId.Value);
                payload[nameof(Event.Generation)].Value<int>().Should().Be(this.testTransEvent.Payload.Generation);
                payload[nameof(Event.ColorScheme)].ToObject<ColorScheme>().Should().Be(this.testTransEvent.Payload.ColorScheme);
            }
        }

        public class DeserializeSpec : MessageSerializerSpec
        {
            [It(nameof(MessageSerializer.Deserialize))]
            public void Should_return_an_error_if_message_version_is_not_supported()
            {
                var json = $@"
                {{
                    ""CorrelationId"": ""{Guid.NewGuid()}"",
                    ""Type"": ""{nameof(SchemePublishedEvent)}"",
                    ""Version"": ""0.0.0"",
                    ""RequestTimestamp"": ""{DateTime.UtcNow}"",
                    ""Payload"": {{}}
                }}";

                var result = this.messageSerializer.Deserialize(json, DateTime.UtcNow);

                result.HasError.Should().BeTrue();
                result.ErrorMessage.Should().Contain(nameof(SchemePublishedEvent));
                result.ErrorMessage.Should().Contain("0.0.0");
            }

            [It(nameof(MessageSerializer.Deserialize))]
            public void Should_return_an_error_if_message_has_incorrect_json_format()
            {
                var json = $@"
                {{
                    ""CorrelationId"": ""not a GUID"",
                    ""Type"": ""{nameof(SchemePublishedEvent)}"",
                    ""Version"": ""0.0.0"",
                    ""RequestTimestamp"": ""not a DateTime"",
                    ""Payload"": {{}}
                }}";

                var result = this.messageSerializer.Deserialize(json, DateTime.UtcNow);

                result.HasError.Should().BeTrue();
                result.Exception.Should().NotBeNull();
            }

            [It(nameof(MessageSerializer.Deserialize))]
            public void Should_correctly_Deserialize_event()
            {
                var te = this.testTransEvent;
                te.Payload.Generation = 3;
                var json = $@"
                {{
                    ""CorrelationId"": ""{te.CorrelationId}"",
                    ""Type"": ""{nameof(SchemePublishedEvent)}"",
                    ""Version"": ""1.0.0"",
                    ""RequestTimestamp"": {JsonConvert.SerializeObject(te.RequestTimestamp)},
                    ""UserId"": ""{te.UserId}"",
                    ""Payload"": {{
                        ""Id"": ""{te.Payload.Id}"",
                        ""AggregateId"": ""{te.Payload.AggregateId}"",
                        ""Generation"": {te.Payload.Generation},
                        ""ColorScheme"": {te.Payload.ColorScheme}
                    }}
                }}";
                new Deserializers.SchemePublishedEventDeserializer_v1_0()
                    .StartAdvancingToTheLatestVersion(te.Payload);

                var result = this.messageSerializer.Deserialize(json, DateTime.UtcNow);

                result.HasError.Should().BeFalse();

                var msg = result.Message;
                msg.DeserializerType.Should().Be<Deserializers.SchemePublishedEventDeserializer_v1_0>();
                msg.CorrelationId.Should().Be(te.CorrelationId);
                msg.RequestTimestamp.Should().Be(te.RequestTimestamp);
                msg.UserId.Should().Be(te.UserId);

                msg.Payload.Should().BeOfType<SchemePublishedEvent>();

                var e = msg.Payload as SchemePublishedEvent;
                e.Id.Should().Be(te.Payload.Id);
                e.AggregateId.Should().Be(te.Payload.AggregateId);
                e.Generation.Should().Be(te.Payload.Generation);
                e.ColorScheme.Should().Be(te.Payload.ColorScheme);
            }
        }
    }
}
