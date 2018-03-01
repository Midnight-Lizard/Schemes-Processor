using MidnightLizard.Commons.Domain.Messaging;
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
    public class SchemesEventStore : DomainEventStore<PublicSchemeId>
    {
        protected override string IndexName => config.ELASTIC_SEARCH_EVENT_STORE_SCHEMES_INDEX_NAME;

        public SchemesEventStore(ElasticSearchConfig config, IMessageSerializer messageSerializer) : base(config, messageSerializer)
        {
        }
    }
}
