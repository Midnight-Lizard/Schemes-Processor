using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common.Interfaces
{
    public interface IAggregateOffset<TAggregateId> : IIdentified<TAggregateId>
         where TAggregateId : DomainEntityId
    {
        int Offset { get; }
    }
}
