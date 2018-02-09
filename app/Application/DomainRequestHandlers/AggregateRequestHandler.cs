using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Messaging;
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
    public abstract class AggregateRequestHandler<TAggregate, TRequest, TAggregateId> :
        IRequestHandler<TRequest, DomainResult>
        where TRequest : DomainRequest<TAggregateId>
        where TAggregate : AggregateRoot<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        protected readonly IMapper mapper;
        protected readonly IOptions<AggregatesConfig> aggregatesConfig;
        protected readonly IMemoryCache memoryCache;
        protected readonly IDomainEventsDispatcher<TAggregateId> eventsDispatcher;
        protected readonly IAggregateSnapshot<TAggregate, TAggregateId> aggregateSnapshot;
        protected readonly IDomainEventsAccessor<TAggregateId> eventsAccessor;

        protected AggregateRequestHandler(
            IMapper mapper,
            IOptions<AggregatesConfig> aggConfig,
            IMemoryCache memoryCache,
            IDomainEventsDispatcher<TAggregateId> eventsDispatcher,
            IAggregateSnapshot<TAggregate, TAggregateId> aggregateSnapshot,
            IDomainEventsAccessor<TAggregateId> eventsAccessor)
        {
            this.mapper = mapper;
            this.aggregatesConfig = aggConfig;
            this.memoryCache = memoryCache;
            this.eventsDispatcher = eventsDispatcher;
            this.aggregateSnapshot = aggregateSnapshot;
            this.eventsAccessor = eventsAccessor;
        }

        protected abstract void HandleDomainRequest(TAggregate aggregate, TRequest request, CancellationToken cancellationToken);

        public virtual async Task<DomainResult> Handle(TRequest request, CancellationToken cancellationToken
            )
        {
            var someResult = await this.GetAggregate(request.AggregateId);
            if (someResult.HasError) return someResult;
            if (someResult is AggregateResult<TAggregate, TAggregateId> aggResult)
            {
                var aggregate = aggResult.Aggregate;

                this.HandleDomainRequest(aggregate, request, cancellationToken);

                var dispatchResults = await this.DispatchDomainEvents(aggregate);
                var error = dispatchResults.Values.FirstOrDefault(result => result.HasError);
                if (error != null)
                {
                    this.memoryCache.Remove(aggregate.Id);
                    return error;
                }
                return DomainResult.Ok;

            }
            return new DomainResult($"{nameof(GetAggregate)} returned a wrong type of result: {someResult?.GetType().FullName ?? "null"}");
        }

        protected virtual async Task<AggregateResult<TAggregate, TAggregateId>> GetAggregateSnapshot(TAggregateId id
            )
        {
            if (this.aggregatesConfig.Value.AGGREGATES_CACHE_ENABLED &&
                this.memoryCache.TryGetValue(id, out TAggregate aggregate))
            {
                return new AggregateResult<TAggregate, TAggregateId>(aggregate);
            }
            else
            {
                var result = await this.aggregateSnapshot.Read(id);
                if (!result.HasError && this.aggregatesConfig.Value.AGGREGATES_CACHE_ENABLED)
                {
                    this.memoryCache.Set(id, result.Aggregate, new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(this.aggregatesConfig.Value.AGGREGATES_CACHE_SLIDING_EXPIRATION_SECONDS))
                        .SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(this.aggregatesConfig.Value.AGGREGATES_CACHE_ABSOLUTE_EXPIRATION_SECONDS)));
                }
                return result;
            }
        }

        protected virtual async Task<Dictionary<DomainEvent<TAggregateId>, DomainResult>> DispatchDomainEvents(IEventSourced<TAggregateId> aggregate
            )
        {
            bool hasError = false;
            var results = new Dictionary<DomainEvent<TAggregateId>, DomainResult>();
            foreach (var @event in aggregate.ReleaseEvents())
            {
                if (!hasError)
                {
                    var result = await this.eventsDispatcher.DispatchEvent(@event);
                    results.Add(@event, result);
                    if (result.HasError)
                    {
                        hasError = true;
                    }
                    else
                    {
                        aggregate.ApplyEventOffset(@event);
                    }
                }
                else
                {
                    results.Add(@event, DomainResult.Skipped);
                }
            }
            return results;
        }

        protected virtual async Task<DomainResult> GetAggregate(TAggregateId aggregateId
            )
        {
            var snapshotResult = await GetAggregateSnapshot(aggregateId);
            if (snapshotResult.HasError) return snapshotResult;

            var aggregate = snapshotResult.Aggregate;

            var eventsResult = await this.eventsAccessor.Read(aggregate);
            if (eventsResult.HasError) return eventsResult;

            aggregate.ReplayDomainEvents(eventsResult.Events, this.mapper);

            if (eventsResult.Events.Count > this.aggregatesConfig.Value.AGGREGATES_MAX_EVENTS_COUNT && !aggregate.IsNew)
            {
                await aggregateSnapshot.Save(aggregate);
            }

            return snapshotResult;
        }
    }
}
