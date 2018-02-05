using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Processor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Processor.Application.DomainRequestHandlers
{
    public class SchemePublishRequestHandler :
        AggregateRequestHandler<PublicScheme, SchemePublishRequest, PublicSchemeId>
    {
        protected SchemePublishRequestHandler(
            IOptions<AggregatesCacheConfig> cacheConfig,
            IMemoryCache memoryCache,
            IDomainEventsDispatcher<PublicSchemeId> domainEventsDispatcher,
            IAggregateSnapshot<PublicScheme, PublicSchemeId> schemesSnapshot,
            IDomainEventsAccessor<PublicSchemeId> eventsAccessor) :
            base(cacheConfig, memoryCache, domainEventsDispatcher, schemesSnapshot, eventsAccessor)
        {
        }

        public async override Task<DomainResult> Handle(SchemePublishRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
