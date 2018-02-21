using Autofac;
using Elasticsearch.Net;
using FluentAssertions;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using MidnightLizard.Schemes.Infrastructure.AutofacModules;
using MidnightLizard.Schemes.Infrastructure.Configuration;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common;
using MidnightLizard.Schemes.Testing;
using Nest;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TransEvent = MidnightLizard.Schemes.Domain.Common.Messaging.TransportMessage<MidnightLizard.Schemes.Domain.Common.Messaging.DomainEvent<MidnightLizard.Schemes.Domain.PublicSchemeAggregate.PublicSchemeId>, MidnightLizard.Schemes.Domain.PublicSchemeAggregate.PublicSchemeId>;
using ITransEvent = MidnightLizard.Schemes.Domain.Common.Messaging.ITransportMessage<MidnightLizard.Schemes.Domain.Common.Messaging.BaseMessage>;

namespace MidnightLizard.Schemes.Infrastructure.EventStore
{
    public class DomainEventStoreSpec : DomainEventStore<PublicSchemeId>
    {
        protected override string IndexName => "test";
        private ITransEvent resultTransEvent;
        private readonly TransEvent testTransEvent = new TransEvent(
                   new SchemePublishedEvent(
                       new PublicSchemeId(Guid.NewGuid()),
                       new PublisherId(Guid.NewGuid()),
                       ColorSchemeSpec.CorrectColorScheme),
                   Guid.NewGuid(), DateTime.UtcNow);

        public DomainEventStoreSpec() : base(
            Substitute.For<ElasticSearchConfig>(),
            Substitute.For<IMessageSerializer>())
        {
        }

        protected override ElasticClient CreateElasticClient()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<MessageSerializationModule>();
            var container = builder.Build();
            var messageSerializer = container.Resolve<IMessageSerializer>();

            return new ElasticClient(InitMapping(new ConnectionSettings(
                new SingleNodeConnectionPool(new Uri("http://test.com")), new InMemoryConnection(),
                (builtin, settings) => new DomainEventSerializer(messageSerializer))
                    .EnableDebugMode(x =>
                    {
                        if (x.RequestBodyInBytes != null)
                        {
                            var body = Encoding.UTF8.GetString(x.RequestBodyInBytes);
                            var doc = JObject.Parse(body)["doc"].ToString();
                            this.resultTransEvent = messageSerializer.Deserialize(doc).Message;
                        }
                    })
            ));
        }

        public class GetEventsSpec : DomainEventStoreSpec
        {
            [It(nameof(GetEvents))]
            public void Should()
            {
                true.Should().BeTrue();
            }
        }

        public class InitMappingSpec : DomainEventStoreSpec
        {
            [It(nameof(InitMapping))]
            public void Should_set_up_DefaultMappingFor_current_Event_type()
            {
                var cs = Substitute.For<ConnectionSettings>(new InMemoryConnection());

                this.InitMapping(cs);

                cs.ReceivedWithAnyArgs(1)
                    .DefaultMappingFor<TransportMessage<DomainEvent<PublicSchemeId>, PublicSchemeId>>(map => map);

            }
        }

        public class SaveEventSpec : DomainEventStoreSpec
        {
            [It(nameof(SaveEvent))]
            public async Task Should_correctly_serialize_Event()
            {
                var result = await this.SaveEvent(this.testTransEvent);

                this.resultTransEvent.CorrelationId.Should().Be(this.testTransEvent.CorrelationId);
                this.resultTransEvent.RequestTimestamp.Should().Be(this.testTransEvent.RequestTimestamp);

                var testPayload = this.testTransEvent.Payload as SchemePublishedEvent;
                var resultPayload = this.resultTransEvent.Payload as SchemePublishedEvent;

                resultPayload.AggregateId.Should().Be(testPayload.AggregateId);
                resultPayload.Id.Should().Be(testPayload.Id);
                resultPayload.PublisherId.Should().Be(testPayload.PublisherId);
                resultPayload.Generation.Should().Be(testPayload.Generation);
                resultPayload.ColorScheme.Should().Be(testPayload.ColorScheme);
            }
        }
    }
}
