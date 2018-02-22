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
using JsonDiffPatchDotNet;

namespace MidnightLizard.Schemes.Infrastructure.EventStore
{
    public abstract class DomainEventStoreSpec : DomainEventStore<PublicSchemeId>
    {
        protected override string IndexName => "test";
        protected abstract void OnRequestCompleted(IApiCallDetails x);
        private IMessageSerializer realMessageSerializer;
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
            this.realMessageSerializer = container.Resolve<IMessageSerializer>();

            return new ElasticClient(InitMapping(new ConnectionSettings(
                new SingleNodeConnectionPool(new Uri("http://test.com")), new InMemoryConnection(),
                (builtin, settings) => new DomainEventSerializer(this.realMessageSerializer))
                    .EnableDebugMode(OnRequestCompleted)
            ));
        }

        public class CreateIndexSpec : DomainEventStoreSpec
        {
            private readonly JObject createIndexCommandSnapshot = JObject.Parse("{\r\n  \"settings\": {\r\n    \"index.number_of_replicas\": 1,\r\n    \"index.number_of_shards\": 2\r\n  },\r\n  \"mappings\": {\r\n    \"event\": {\r\n      \"_routing\": {\r\n        \"required\": true\r\n      },\r\n      \"properties\": {\r\n        \"Type\": {\r\n          \"type\": \"keyword\"\r\n        },\r\n        \"Version\": {\r\n          \"type\": \"keyword\"\r\n        },\r\n        \"CorrelationId\": {\r\n          \"type\": \"keyword\"\r\n        },\r\n        \"RequestTimestamp\": {\r\n          \"type\": \"date\"\r\n        },\r\n        \"Payload\": {\r\n          \"type\": \"object\",\r\n          \"properties\": {\r\n            \"Generation\": {\r\n              \"type\": \"integer\"\r\n            },\r\n            \"AggregateId\": {\r\n              \"type\": \"keyword\"\r\n            },\r\n            \"Id\": {\r\n              \"type\": \"keyword\"\r\n            }\r\n          }\r\n        }\r\n      }\r\n    }\r\n  }\r\n}");
            private JObject createIndexCommand;

            protected override void OnRequestCompleted(IApiCallDetails x)
            {
                if (x.RequestBodyInBytes != null && x.RequestBodyInBytes.Length > 1)
                {
                    this.createIndexCommand = JObject.Parse(Encoding.UTF8.GetString(x.RequestBodyInBytes));
                }
            }

            [It(nameof(CreateIndex))]
            public void Should_issue_correct_CreateIndex_command()
            {
                this.CreateIndex();
                if (!JToken.DeepEquals(createIndexCommand, createIndexCommandSnapshot))
                {
                    new JsonDiffPatch()
                        .Diff(createIndexCommandSnapshot, createIndexCommand)
                        .ToString().Should().BeEmpty();
                }
            }
        }

        public class GetEventsSpec : DomainEventStoreSpec
        {
            private JObject command;

            [It(nameof(GetEvents))]
            public void Should_filter_events_older_than_provided_generation()
            {
                var generation = 42;
                var result = this.GetEvents(this.testTransEvent.Payload.AggregateId, generation);
                this.command.SelectToken("..range['Payload.Generation'].gt")
                    .Value<int>().Should().Be(generation);
            }

            [It(nameof(GetEvents))]
            public void Should_filter_events_by_provided_AggregateId()
            {
                var result = this.GetEvents(this.testTransEvent.Payload.AggregateId, 0);
                this.command.SelectToken("..term['Payload.AggregateId'].value")
                    .ToObject<Guid>().Should().Be(this.testTransEvent.Payload.AggregateId.Value);
            }

            protected override void OnRequestCompleted(IApiCallDetails x)
            {
                if (x.RequestBodyInBytes != null && x.RequestBodyInBytes.Length > 1)
                {
                    var body = Encoding.UTF8.GetString(x.RequestBodyInBytes);
                    this.command = JObject.Parse(body);
                }
            }
        }

        public class InitMappingSpec : DomainEventStoreSpec
        {
            protected override void OnRequestCompleted(IApiCallDetails x) { }

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
            private ITransEvent resultTransEvent;

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

            protected override void OnRequestCompleted(IApiCallDetails x)
            {
                if (x.RequestBodyInBytes != null && x.RequestBodyInBytes.Length > 1)
                {
                    var body = Encoding.UTF8.GetString(x.RequestBodyInBytes);
                    var doc = JObject.Parse(body)["doc"].ToString();
                    this.resultTransEvent = this.realMessageSerializer.Deserialize(doc).Message;
                }
            }
        }
    }
}
