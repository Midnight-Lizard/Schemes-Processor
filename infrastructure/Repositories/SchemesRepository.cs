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
            this.config = config;

            var node = new Uri(config.ELASTIC_SEARCH_CLIENT_URL);

            elasticClient = new ElasticClient(
                new ConnectionSettings(node)
                    .DefaultIndex("schemes")
                    .InferMappingFor<SchemeAggregateRoot>(map => map
                        .IdProperty(to => to.Id)
                        .IndexName("snapshot")
                        .TypeName("scheme"))
            );
        }

        public void Save(SchemeAggregateRoot scheme)
        {
            this.elasticClient.Update<SchemeAggregateRoot, object>(
                new DocumentPath<SchemeAggregateRoot>(scheme.Id),
                u => u
                    .Doc(new { scheme.PublisherId })
                    .DocAsUpsert());
        }
    }
}