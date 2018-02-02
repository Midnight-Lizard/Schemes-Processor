using MediatR;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
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
        where TAggregateId : EntityId
    {
        protected readonly IDomainEventsDispatcher<TAggregateId> domainEventsDispatcher;
        private readonly IDomainEventsAccessor<TAggregateId> eventsAccessor;

        protected AggregateRequestHandler(
            IDomainEventsDispatcher<TAggregateId> domainEventsDispatcher,
            IDomainEventsAccessor<TAggregateId> eventsAccessor)
        {
            this.domainEventsDispatcher = domainEventsDispatcher;
            this.eventsAccessor = eventsAccessor;
        }

        protected virtual async Task<List<DomainResult>> DispatchDomainEvents(TAggregate aggregate)
        {
            var results = new List<DomainResult>();
            foreach (var @event in aggregate.Events)
            {
                results.Add(await this.domainEventsDispatcher.DispatchEvent(@event));
            }
            return results;
        }

        protected virtual async Task<DomainEventsResult<TAggregateId>> ReadDomainEvents(TAggregateId id, int offset)
        {
            return await this.eventsAccessor.Read(id, offset);
        }

        protected abstract Task<AggregateResult<TAggregate>> GetAggregate(TAggregateId id);
        public abstract Task<DomainResult> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
