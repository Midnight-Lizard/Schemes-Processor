using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common.Results
{
    public class DomainEventsResult<TAggregateId> : DomainResult
        where TAggregateId : DomainEntityId
    {
        public List<DomainEvent<TAggregateId>> Events { get; }

        public DomainEventsResult() { }

        public DomainEventsResult(List<DomainEvent<TAggregateId>> events)
        {
            Events = events;
        }

        public DomainEventsResult(string errorMessage) : base(errorMessage) { }

        public DomainEventsResult(Exception ex) : base(ex) { }

        public DomainEventsResult(bool hasError, Exception ex, string errorMessage)
            : base(hasError, ex, errorMessage) { }
    }
}
