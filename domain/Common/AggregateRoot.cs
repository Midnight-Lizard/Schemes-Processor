using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class AggregateRoot<TId> : DomainEntity<TId>
        where TId : DomainEntityId
    {
    }
}
