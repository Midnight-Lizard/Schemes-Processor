using MediatR;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MidnightLizard.Schemes.Domain.Common.Messaging
{
    public abstract class DomainEvent<TAggregateId> : DomainMessage<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        public int RequestOffset { get; protected set; }

        public DomainEvent<TAggregateId> SetUp(TAggregateId aggregateId, Guid? correlationId = null)
        {
            this.Id = Guid.NewGuid();
            this.AggregateId = aggregateId;
            this.CorrelationId = correlationId ?? Guid.NewGuid();
            return this;
        }
    }
}
