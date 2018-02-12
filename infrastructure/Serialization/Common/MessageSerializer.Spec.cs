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

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Common
{
    public class MessageSerializerSpec
    {
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
            private readonly TransportMessage<SchemePublishedEvent, PublicSchemeId> testTransEvent =
                new TransportMessage<SchemePublishedEvent, PublicSchemeId>(
                    new SchemePublishedEvent(new PublicSchemeId(), new PublisherId(), new ColorScheme()),
                    Guid.NewGuid(), DateTime.Now);

            [It(nameof(MessageSerializer.Serialize))]
            public void Should_serialize()
            {
                var json = this.messageSerializer.Serialize(this.testTransEvent);
                json.Should().NotBeEmpty();
                var message = this.messageSerializer.Deserialize(json);
                message.HasError.Should().BeFalse();
            }
        }
    }
}
