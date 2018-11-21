using Elasticsearch.Net;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Schemes.Infrastructure.Configuration;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MidnightLizard.Schemes.Infrastructure.Versioning;

namespace MidnightLizard.Schemes.Infrastructure.Snapshot
{
    public abstract class AggregateSnapshotAccessor<TAggregate, TAggregateId> : IAggregateSnapshotAccessor<TAggregate, TAggregateId>
        where TAggregateId : DomainEntityId
        where TAggregate : AggregateRoot<TAggregateId>
    {
        protected abstract string IndexName { get; }
        protected readonly IElasticClient elasticClient;
        protected readonly SchemaVersion version;
        protected readonly ElasticSearchConfig config;

        public AggregateSnapshotAccessor(SchemaVersion version, ElasticSearchConfig config)
        {
            this.version = version;
            this.config = config;
            this.elasticClient = CreateElasticClient();
            CheckIndexExists();
        }

        protected virtual void CheckIndexExists()
        {
            if ((this.elasticClient.IndexExists(this.IndexName)).Exists == false)
            {
                CreateIndex();
            }
        }

        protected virtual void CreateIndex()
        {
            this.elasticClient
                .CreateIndex(this.IndexName, ix => ix
                    .Mappings(ApplyAggregateMappingsOnIndex)
                    .Settings(set => set
                        .NumberOfShards(this.config.ELASTIC_SEARCH_SNAPSHOT_SHARDS)
                        .NumberOfReplicas(this.config.ELASTIC_SEARCH_SNAPSHOT_REPLICAS)));
        }

        protected abstract IPromise<IMappings> ApplyAggregateMappingsOnIndex(MappingsDescriptor md);

        protected virtual IElasticClient CreateElasticClient()
        {
            var node = new Uri(config.ELASTIC_SEARCH_CLIENT_URL);
            return new ElasticClient(ApplyAggregateMappingsOnConnection(new ConnectionSettings(
                new SingleNodeConnectionPool(node),
                (builtin, settings) => new AggregateSerializer())));
        }

        protected virtual ConnectionSettings ApplyAggregateMappingsOnConnection(ConnectionSettings connectionSettings)
        {
            return connectionSettings
               .DefaultFieldNameInferrer(i => i)
               .DefaultMappingFor<TAggregate>(map => map
                   .IdProperty(to => to.Id)
                   .IndexName(this.IndexName)
                   .TypeName("snapshot"));
        }

        public async Task<AggregateSnapshot<TAggregate, TAggregateId>> Read(TAggregateId id)
        {
            var result = await this.elasticClient
                .GetAsync<TAggregate>(new DocumentPath<TAggregate>(id.ToString()));

            if (result.IsValid &&
                result.Fields.Value<string>(nameof(Version)) == this.version.ToString())
            {
                var requestTimestampField = nameof(AggregateSnapshot<TAggregate, TAggregateId>.RequestTimestamp);

                return new AggregateSnapshot<TAggregate, TAggregateId>(result.Source,
                    result.Fields.Value<DateTime>(requestTimestampField));
            }
            return new AggregateSnapshot<TAggregate, TAggregateId>(CreateNewAggregate(id), DateTime.MinValue);
        }

        protected abstract TAggregate CreateNewAggregate(TAggregateId id);

        public abstract Task Save(AggregateSnapshot<TAggregate, TAggregateId> aggregate);
    }
}
