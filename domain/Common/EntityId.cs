using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class EntityId<T>
    {
        public T Value { get; protected set; }
    }
}
