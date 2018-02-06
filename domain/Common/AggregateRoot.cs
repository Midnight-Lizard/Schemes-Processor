using AutoMapper;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public abstract class AggregateRoot<TAggregateId> :
        DomainEntity<TAggregateId>, IAggregateOffset<TAggregateId>, IEventSourced<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        private readonly List<DomainEvent<TAggregateId>> events = new List<DomainEvent<TAggregateId>>();

        public int EventOffset { get; protected set; }
        public int RequestOffset { get; protected set; }

        public abstract void Reduce(DomainEvent<TAggregateId> @event, IMapper mapper);

        public virtual IEnumerable<DomainEvent<TAggregateId>> ReleaseEvents()
        {
            var events = this.events.ToArray();
            this.events.Clear();
            return events;
        }

        public virtual void ReplayDomainEvents(IEnumerable<DomainEvent<TAggregateId>> events, IMapper mapper)
        {
            foreach (var e in events)
            {
                this.Reduce(e, mapper);
                this.EventOffset = e.Offset;
                this.RequestOffset = e.RequestOffset;
            }
        }
    }
}
