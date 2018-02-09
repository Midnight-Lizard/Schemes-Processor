using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Results;
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
            IMapper mapper,
            IOptions<AggregatesConfig> cacheConfig,
            IMemoryCache memoryCache,
            IDomainEventsDispatcher<PublicSchemeId> domainEventsDispatcher,
            IAggregateSnapshot<PublicScheme, PublicSchemeId> schemesSnapshot,
            IDomainEventsAccessor<PublicSchemeId> eventsAccessor) :
            base(mapper, cacheConfig, memoryCache, domainEventsDispatcher, schemesSnapshot, eventsAccessor)
        {
        }

        protected override void HandleDomainRequest(PublicScheme aggregate, SchemePublishRequest request, CancellationToken cancellationToken)
        {
            aggregate.Publish(request.PublisherId, request.CorrelationId, request);
        }
    }
}
