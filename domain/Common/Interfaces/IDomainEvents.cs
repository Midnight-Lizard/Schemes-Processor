using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common.Interfaces
{
    public interface IDomainEvents<TId> : IIdentified<TId>
        where TId : DomainEntityId
    {
        List<DomainEvent<TId>> Events { get; }
    }
}
