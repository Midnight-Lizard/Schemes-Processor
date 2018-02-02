using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class DomainEventsResult<TAggregateId> : DomainResult
        where TAggregateId : EntityId
    {
        public List<DomainEvent<TAggregateId>> Events { get; }

        public DomainEventsResult() { }

        public DomainEventsResult(List<DomainEvent<TAggregateId>> events)
        {
            Events = events;
        }

        public DomainEventsResult(Exception ex) : base(ex)
        {
        }

        public DomainEventsResult(bool hasError, Exception ex, string errorMessage) : base(hasError, ex, errorMessage)
        {
        }
    }
}
