using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common.Interfaces
{
    public interface IIdentified<TId> where TId : DomainEntityId
    {
        TId Id { get; }
    }
}
