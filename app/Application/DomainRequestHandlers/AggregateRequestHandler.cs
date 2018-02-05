using MediatR;
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
    public abstract class AggregateRequestHandler<TAggregate, TRequest, TAggregateId> :
        IRequestHandler<TRequest, DomainResult>
        where TRequest : DomainRequest<TAggregateId>
        where TAggregate : AggregateRoot<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        protected readonly IOptions<AggregatesCacheConfig> cacheConfig;
        protected readonly IMemoryCache memoryCache;
        protected readonly IDomainEventsDispatcher<TAggregateId> eventsDispatcher;
        protected readonly IAggregateSnapshot<TAggregate, TAggregateId> aggregateSnapshot;
        protected readonly IDomainEventsAccessor<TAggregateId> eventsAccessor;

        protected AggregateRequestHandler(
            IOptions<AggregatesCacheConfig> cacheConfig,
            IMemoryCache memoryCache,
            IDomainEventsDispatcher<TAggregateId> eventsDispatcher,
            IAggregateSnapshot<TAggregate, TAggregateId> aggregateSnapshot,
            IDomainEventsAccessor<TAggregateId> eventsAccessor)
        {
            this.cacheConfig = cacheConfig;
            this.memoryCache = memoryCache;
            this.eventsDispatcher = eventsDispatcher;
            this.aggregateSnapshot = aggregateSnapshot;
            this.eventsAccessor = eventsAccessor;
        }

        public abstract Task<DomainResult> Handle(TRequest request, CancellationToken cancellationToken);

        protected virtual async Task<AggregateResult<TAggregate>> GetAggregateSnapshot(TAggregateId id
            )
        {
            if (this.cacheConfig.Value.AGGREGATES_CACHE_ENABLED &&
                this.memoryCache.TryGetValue(id, out TAggregate aggregate))
            {
                return new AggregateResult<TAggregate>(aggregate);
            }
            else
            {
                var result = await this.aggregateSnapshot.Read(id);
                if (!result.HasError && this.cacheConfig.Value.AGGREGATES_CACHE_ENABLED)
                {
                    this.memoryCache.Set(id, result.Aggregate, new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(this.cacheConfig.Value.AGGREGATES_CACHE_SLIDING_EXPIRATION_SECONDS))
                        .SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(this.cacheConfig.Value.AGGREGATES_CACHE_ABSOLUTE_EXPIRATION_SECONDS)));
                }
                return result;
            }
        }

        protected virtual async Task<Dictionary<DomainEvent<TAggregateId>, DomainResult>> DispatchDomainEvents(TAggregate aggregate
            )
        {
            var results = new Dictionary<DomainEvent<TAggregateId>, DomainResult>();
            foreach (var @event in aggregate.Events)
            {
                var result = await this.eventsDispatcher.DispatchEvent(@event);
                results.Add(@event, result);
                if (result.HasError)
                {
                    break;
                }
            }
            return results;
        }

        protected virtual async Task<DomainEventsResult<TAggregateId>> ReadDomainEvents(TAggregateId id, int offset
            )
        {
            return await this.eventsAccessor.Read(id, offset);
        }
    }
}
