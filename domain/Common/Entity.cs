using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class Entity<TId> where TId : EntityId
    {
        public TId Id { get; set; }
    }
}
