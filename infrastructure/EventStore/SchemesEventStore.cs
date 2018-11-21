using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Infrastructure.Configuration;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common;

namespace MidnightLizard.Schemes.Infrastructure.EventStore
{
    public class SchemesEventStore : DomainEventStore<PublicSchemeId>
    {
        protected override string IndexName => this.config.ELASTIC_SEARCH_EVENT_STORE_SCHEMES_INDEX_NAME;

        public SchemesEventStore(ElasticSearchConfig config, IMessageSerializer messageSerializer) :
            base(config, messageSerializer)
        {
        }
    }
}
