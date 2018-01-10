using System;
using MidnightLizard.Schemes.Domain.Scheme;
using MidnightLizard.Schemes.Infrastructure.Configuration;
using Nest;

namespace MidnightLizard.Schemes.Infrastructure.Repositories
{
    public class SchemesRepository : ISchemesRepository
    {
        private readonly ElasticSearchConfig config;
        private readonly ElasticClient elasticClient;

        public SchemesRepository(ElasticSearchConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            var node = new Uri(config.ELASTIC_SEARCH_CLIENT_URL);

            elasticClient = new ElasticClient(
                new ConnectionSettings(node)
                    .InferMappingFor<SchemeAggregateRoot>(map => map
                        .IdProperty(to => to.Id)
                        .IndexName("scheme-snapshots")
                        .TypeName("scheme"))
            );
        }

        public void Save(SchemeAggregateRoot scheme)
        {
            this.elasticClient.Update<SchemeAggregateRoot, object>(
                new DocumentPath<SchemeAggregateRoot>(scheme.Id),
                u => u
                    .Doc(new { scheme.PublisherId, scheme.ColorScheme })
                    .DocAsUpsert());
        }
    }
}