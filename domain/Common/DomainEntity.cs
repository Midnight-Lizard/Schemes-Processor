using MidnightLizard.Schemes.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class DomainEntity<TId> : IIdentified<TId>, IDomainEvents<TId>
        where TId : DomainEntityId
    {
        public TId Id { get; protected set; }
        public List<DomainEvent<TId>> Events { get; } = new List<DomainEvent<TId>>();
    }
}
