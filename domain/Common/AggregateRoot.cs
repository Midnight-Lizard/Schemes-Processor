using MidnightLizard.Schemes.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class AggregateRoot<TAggregateId> : 
        DomainEntity<TAggregateId>, IAggregateOffset<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        public int Offset { get; protected set; }
    }
}
