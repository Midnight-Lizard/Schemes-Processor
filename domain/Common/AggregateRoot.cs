using AutoMapper;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public abstract class AggregateRoot<TAggregateId> : DomainEntity<TAggregateId>, IEventSourced<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        private readonly List<DomainEvent<TAggregateId>> pendingEvents = new List<DomainEvent<TAggregateId>>();

        public virtual int Generation { get; protected set; }
        protected bool isNew;
        public virtual bool IsNew() => isNew;
        public abstract Version Version();

        public abstract void Reduce(DomainEvent<TAggregateId> @event);

        public AggregateRoot() { }

        /// <summary>
        /// special ctor for new instances
        /// </summary>
        /// <param name="isNew">true</param>
        public AggregateRoot(bool isNew)
        {
            if (!isNew) throw new ArgumentException(nameof(isNew), "Should be always new");
            this.isNew = isNew;
        }

        public virtual IEnumerable<DomainEvent<TAggregateId>> ReleaseEvents()
        {
            var events = this.pendingEvents.ToArray();
            this.pendingEvents.Clear();
            return events;
        }

        public virtual void AddDomainEvent(DomainEvent<TAggregateId> @event)
        {
            @event.Generation = this.Generation + 1;
            this.Reduce(@event);
            this.Generation = @event.Generation;
            this.pendingEvents.Add(@event);
        }

        public virtual void ReplayDomainEvents(IEnumerable<DomainEvent<TAggregateId>> events)
        {
            foreach (var @event in events)
            {
                this.Reduce(@event);
                this.Generation = @event.Generation;
            }
        }
    }
}
