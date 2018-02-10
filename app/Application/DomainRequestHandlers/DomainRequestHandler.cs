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
    public abstract class DomainRequestHandler<TAggregate, TRequest, TAggregateId> :
        IRequestHandler<TransportMessage<TRequest, TAggregateId>, DomainResult>
        where TRequest : DomainRequest<TAggregateId>
        where TAggregate : AggregateRoot<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        protected readonly IOptions<AggregatesConfig> aggregatesConfig;
        protected readonly IMemoryCache memoryCache;
        protected readonly IDomainEventsDispatcher<TAggregateId> eventsDispatcher;
        protected readonly IAggregateSnapshotAccessor<TAggregate, TAggregateId> aggregateSnapshotAccessor;
        protected readonly IDomainEventsAccessor<TAggregateId> eventsAccessor;

        protected DomainRequestHandler(
            IOptions<AggregatesConfig> aggConfig,
            IMemoryCache memoryCache,
            IDomainEventsDispatcher<TAggregateId> eventsDispatcher,
            IAggregateSnapshotAccessor<TAggregate, TAggregateId> aggregateSnapshot,
            IDomainEventsAccessor<TAggregateId> eventsAccessor)
        {
            this.aggregatesConfig = aggConfig;
            this.memoryCache = memoryCache;
            this.eventsDispatcher = eventsDispatcher;
            this.aggregateSnapshotAccessor = aggregateSnapshot;
            this.eventsAccessor = eventsAccessor;
        }

        protected abstract void
            HandleDomainRequest(TAggregate aggregate, TRequest request, CancellationToken cancellationToken);

        public virtual async Task<DomainResult>
            Handle(TransportMessage<TRequest, TAggregateId> transRequest, CancellationToken cancellationToken
            )
        {
            var someResult = await this.GetAggregate(transRequest.Message.AggregateId);
            if (someResult.HasError) return someResult;
            if (someResult is AggregateSnapshotResult<TAggregate, TAggregateId> aggregateSnapshotResult)
            {
                var aggregateSnapshot = aggregateSnapshotResult.AggregateSnapshot;

                if (aggregateSnapshot.Aggregate.IsNew() || aggregateSnapshot.RequestTimestamp < transRequest.RequestTimestamp)
                {
                    this.HandleDomainRequest(aggregateSnapshot.Aggregate, transRequest.Message, cancellationToken);

                    var dispatchResults = await this.DispatchDomainEvents(aggregateSnapshot.Aggregate, transRequest);
                    var error = dispatchResults.Values.FirstOrDefault(result => result.HasError);
                    if (error != null)
                    {
                        this.memoryCache.Remove(aggregateSnapshot.Aggregate.Id);
                        return error;
                    }
                    else
                    {
                        aggregateSnapshot.RequestTimestamp = transRequest.RequestTimestamp;
                    }
                }
                return DomainResult.Ok;
            }
            return new DomainResult($"{nameof(GetAggregate)} returned a wrong type of result: {someResult?.GetType().FullName ?? "null"}");
        }

        protected virtual async Task<AggregateSnapshot<TAggregate, TAggregateId>>
            GetAggregateSnapshot(TAggregateId id
            )
        {
            if (this.aggregatesConfig.Value.AGGREGATE_CACHE_ENABLED &&
                this.memoryCache.TryGetValue(id, out AggregateSnapshot<TAggregate, TAggregateId> aggregateSnapshot))
            {
                return aggregateSnapshot;
            }
            else
            {
                aggregateSnapshot = await this.aggregateSnapshotAccessor.Read(id);
                if (this.aggregatesConfig.Value.AGGREGATE_CACHE_ENABLED)
                {
                    this.memoryCache.Set(id, aggregateSnapshot, new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(this.aggregatesConfig.Value.AGGREGATE_CACHE_SLIDING_EXPIRATION_SECONDS))
                        .SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(this.aggregatesConfig.Value.AGGREGATE_CACHE_ABSOLUTE_EXPIRATION_SECONDS)));
                }
                return aggregateSnapshot;
            }
        }

        protected virtual async Task<Dictionary<DomainEvent<TAggregateId>, DomainResult>>
            DispatchDomainEvents(TAggregate aggregate, TransportMessage<TRequest, TAggregateId> transRequest
            )
        {
            bool hasError = false;
            var results = new Dictionary<DomainEvent<TAggregateId>, DomainResult>();
            foreach (var @event in aggregate.ReleaseEvents())
            {
                if (!hasError)
                {
                    var result = await this.eventsDispatcher.DispatchEvent(
                        new TransportMessage<DomainEvent<TAggregateId>, TAggregateId>(
                            @event, transRequest.CorrelationId, transRequest.RequestTimestamp));
                    results.Add(@event, result);
                    if (result.HasError)
                    {
                        hasError = true;
                    }
                }
                else
                {
                    results.Add(@event, DomainResult.Skipped);
                }
            }
            return results;
        }

        protected virtual async Task<DomainResult>
            GetAggregate(TAggregateId aggregateId
            )
        {
            var aggregateSnapshot = await GetAggregateSnapshot(aggregateId);

            var eventsResult = await this.eventsAccessor.GetEvents(aggregateId, 0);
            if (eventsResult.HasError) return eventsResult;

            aggregateSnapshot.Aggregate.ReplayDomainEvents(eventsResult.Events.Select(x => x.Message));

            if (eventsResult.Events.Count() > this.aggregatesConfig.Value.AGGREGATE_MAX_EVENTS_COUNT && !aggregateSnapshot.Aggregate.IsNew())
            {
                await this.aggregateSnapshotAccessor.Save(aggregateSnapshot);
            }

            return new AggregateSnapshotResult<TAggregate, TAggregateId>(aggregateSnapshot);
        }
    }
}
