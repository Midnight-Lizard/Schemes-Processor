using MediatR;
using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common.Messaging
{
    public abstract class DomainMessage<TAggregateId> : BaseMessage
        where TAggregateId : DomainEntityId
    {
        public virtual TAggregateId AggregateId { get; protected set; }
    }
}
