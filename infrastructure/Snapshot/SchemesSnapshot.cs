using System;
using System.Threading.Tasks;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using Nest;

namespace MidnightLizard.Schemes.Infrastructure.Snapshot
{
    public class SchemesSnapshot : IAggregateSnapshot<PublicScheme, PublicSchemeId>
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
                        .IndexName("scheme-snapshots")
                        .TypeName("scheme"))
            );
        }

        public Task<AggregateResult<PublicScheme>> Read(PublicSchemeId id)
        {
            throw new NotImplementedException();
        }

        public async Task<DomainResult> Save(PublicScheme scheme)
        {
            var result = await this.elasticClient.UpdateAsync<PublicScheme, object>(
                new DocumentPath<PublicScheme>(scheme.Id.Value),
                u => u.Doc(new
                {
                    scheme.PublisherId,
                    scheme.ColorScheme
                }).DocAsUpsert());
            return new DomainResult(!result.IsValid,
                result.OriginalException,
                result.OriginalException != null
                    ? result.OriginalException.Message
                    : result.ServerError?.Error.Reason);
        }
    }
}