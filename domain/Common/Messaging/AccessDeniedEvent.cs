using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common.Messaging
{
    public abstract class AccessDeniedEvent<TAggregateId> : DomainEvent<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        protected AccessDeniedEvent() { }

        public AccessDeniedEvent(TAggregateId aggregateId) : base(aggregateId)
        {
        }
    }
}
