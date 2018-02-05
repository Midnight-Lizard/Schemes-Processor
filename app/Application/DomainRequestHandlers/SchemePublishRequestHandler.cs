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
            var snapshotResult = await this.GetAggregateSnapshot(request.AggregateId);
            if (snapshotResult.HasError) return snapshotResult;

            var aggregate = snapshotResult.Aggregate;

            var eventsResult = await this.ReadDomainEvents(aggregate);

            // TODO: snapshotResult.Aggregate.ApplyEvents

            // TODO: do something actual

            var dispatchResults = await this.DispatchDomainEvents(aggregate);

            return DomainResult.Ok;
        }
    }
}
