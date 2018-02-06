using MidnightLizard.Schemes.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class DomainEntity<TId> : IIdentified<TId>
        where TId : DomainEntityId
    {
        public TId Id { get; protected set; }
    }
}
