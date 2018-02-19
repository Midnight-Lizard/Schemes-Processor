using MediatR;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Processor.Application.DomainEventHandlers
{
    public abstract class DomainEventHandler<TEvent, TAggregateId>
        : IRequestHandler<TransportMessage<TEvent, TAggregateId>, DomainResult>
        where TEvent : DomainEvent<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        protected readonly IDomainEventStore<TAggregateId> domainEventAccessor;

        public DomainEventHandler(IDomainEventStore<TAggregateId> domainEventAccessor)
        {
            this.domainEventAccessor = domainEventAccessor;
        }

        public virtual async Task<DomainResult> Handle(TransportMessage<TEvent, TAggregateId> @event, CancellationToken cancellationToken)
        {
            var result = await this.domainEventAccessor.SaveEvent(@event);

            return result;
        }
    }
}
