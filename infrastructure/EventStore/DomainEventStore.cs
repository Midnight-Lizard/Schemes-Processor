using Elasticsearch.Net;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.Common.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Infrastructure.Configuration;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.EventStore
{
    public abstract class DomainEventStore<TAggregateId> : IDomainEventStore<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        protected readonly ElasticSearchConfig config;
        protected readonly IMessageSerializer messageSerializer;
        protected readonly ElasticClient elasticClient;
        protected abstract string IndexName { get; }

        public DomainEventStore(ElasticSearchConfig config, IMessageSerializer messageSerializer)
        {
            this.config = config;
            this.messageSerializer = messageSerializer;
            this.elasticClient = CreateElasticClient();
            CheckIndices();
        }

        protected virtual void CheckIndices()
        {
            if ((this.elasticClient.IndexExists(this.IndexName)).Exists == false)
            {
                this.elasticClient
                   .CreateIndex(this.IndexName, ix => ix
                       .Settings(set => set
                           .NumberOfShards(this.config.ELASTIC_SEARCH_SHARDS)
                           .NumberOfReplicas(this.config.ELASTIC_SEARCH_REPLICAS)));
            }
        }

        protected virtual ElasticClient CreateElasticClient()
        {
            var node = new Uri(config.ELASTIC_SEARCH_CLIENT_URL);
            return new ElasticClient(InitMapping(new ConnectionSettings(
                new SingleNodeConnectionPool(node),
                (builtin, settings) => new DomainEventSerializer(messageSerializer))));
        }

        protected virtual ConnectionSettings InitMapping(ConnectionSettings connectionSettings)
        {
            return connectionSettings.DefaultMappingFor<TransportMessage<DomainEvent<TAggregateId>, TAggregateId>>(
                map => map
                    .IdProperty(to => to.Id)
                    .RoutingProperty(x => x.AggregateId)
                    .IndexName(this.IndexName)
                    .TypeName("event"));
        }

        public Task<DomainEventsResult<TAggregateId>> GetEvents(TAggregateId aggregateId, int sinceGeneration)
        {
            throw new NotImplementedException();
        }

        public async Task<DomainResult> SaveEvent(ITransportMessage<DomainEvent<TAggregateId>> @event)
        {
            var result = await this.elasticClient.UpdateAsync<TransportMessage<DomainEvent<TAggregateId>, TAggregateId>, object>(
                new DocumentPath<TransportMessage<DomainEvent<TAggregateId>, TAggregateId>>(@event.Payload.Id),
                u => u.Doc(@event).DocAsUpsert());
            if (result.IsValid != true)
            {
                return new DomainResult(result.IsValid, result.OriginalException, result.ServerError?.Error?.Reason);
            }
            return DomainResult.Ok;
        }
    }
}
