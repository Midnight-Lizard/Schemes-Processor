using MidnightLizard.Schemes.Domain.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common.Results
{
    public class DomainEventsResult<TAggregateId> : DomainResult
        where TAggregateId : DomainEntityId
    {
        public IEnumerable<TransportMessage<DomainEvent<TAggregateId>, TAggregateId>> Events { get; }

        public DomainEventsResult() { }

        public DomainEventsResult(IEnumerable<TransportMessage<DomainEvent<TAggregateId>, TAggregateId>> events)
        {
            Events = events;
        }

        public DomainEventsResult(string errorMessage) : base(errorMessage) { }

        public DomainEventsResult(Exception ex) : base(ex) { }

        public DomainEventsResult(bool hasError, Exception ex, string errorMessage)
            : base(hasError, ex, errorMessage) { }
    }
}
