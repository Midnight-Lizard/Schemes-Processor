using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common.Interfaces
{
    public interface IEventSourced<TAggregateId> : IIdentified<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        void AddDomainEvent(DomainEvent<TAggregateId> @event);
        IEnumerable<DomainEvent<TAggregateId>> ReleaseEvents();
        void ReplayDomainEvents(IEnumerable<DomainEvent<TAggregateId>> events);
        void Reduce(DomainEvent<TAggregateId> @event);
    }
}
