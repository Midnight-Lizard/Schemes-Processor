using System;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Infrastructure;
using Nest;

namespace MidnightLizard.Schemes.Infrastructure.Snapshot
{
    public class SchemesSnapshot : ISchemesSnapshot
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

        public PublicScheme Read(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Save(PublicScheme scheme)
        {
            this.elasticClient.Update<PublicScheme, object>(
                new DocumentPath<PublicScheme>(scheme.Id.Value),
                u => u.Doc(new
                {
                    scheme.PublisherId,
                    scheme.ColorScheme
                }).DocAsUpsert());
        }
    }
}