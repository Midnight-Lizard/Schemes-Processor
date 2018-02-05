using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class DomainEntity<TId> where TId : DomainEntityId
    {
        public TId Id { get; set; }
        public List<DomainEvent<TId>> Events { get; } = new List<DomainEvent<TId>>();
    }
}
