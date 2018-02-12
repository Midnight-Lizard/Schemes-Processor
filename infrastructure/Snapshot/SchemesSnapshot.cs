using System;
using System.Threading.Tasks;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using Nest;

namespace MidnightLizard.Schemes.Infrastructure.Snapshot
{
    public class SchemesSnapshot : IAggregateSnapshotAccessor<PublicScheme, PublicSchemeId>
    {
        private readonly ElasticSearchConfig config;
        private readonly ElasticClient elasticClient;

        public SchemesSnapshot(ElasticSearchConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            var node = new Uri(config.ELASTIC_SEARCH_CLIENT_URL);

            elasticClient = new ElasticClient(
                new ConnectionSettings(node)
                    .InferMappingFor<PublicScheme>(map => map
                        .IdProperty(to => to.Id)
                        .IndexName("scheme-snapshot")
                        .TypeName("scheme"))
            );
        }

        public async Task<AggregateSnapshot<PublicScheme, PublicSchemeId>> Read(PublicSchemeId id)
        {
            var result = await this.elasticClient
                .GetAsync<PublicScheme>(new DocumentPath<PublicScheme>(id.Value));

            if (result.IsValid &&
                result.Fields.Value<Version>(new Field(nameof(Version))) == result.Source.Version())
            {
                var requestTimestampField = new Field(nameof(AggregateSnapshot<PublicScheme, PublicSchemeId>.RequestTimestamp));

                return new AggregateSnapshot<PublicScheme, PublicSchemeId>(result.Source,
                    result.Fields.Value<DateTime>(requestTimestampField));
            }
            return new AggregateSnapshot<PublicScheme, PublicSchemeId>(new PublicScheme(id), DateTime.MinValue);
        }

        public async Task Save(AggregateSnapshot<PublicScheme, PublicSchemeId> schemeSnapshot)
        {
            var result = await this.elasticClient.UpdateAsync<PublicScheme, object>(
                new DocumentPath<PublicScheme>(schemeSnapshot.Aggregate.Id.Value),
                u => u.Doc(new
                {
                    schemeSnapshot.Aggregate.PublisherId,
                    schemeSnapshot.Aggregate.ColorScheme,
                    schemeSnapshot.RequestTimestamp,
                    Version = schemeSnapshot.Aggregate.Version()
                }).DocAsUpsert());
        }
    }
}