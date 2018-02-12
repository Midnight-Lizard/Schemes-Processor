using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public abstract class DomainEntityId { }

    public class DomainEntityId<T> : DomainEntityId
        where T : IComparable, IComparable<T>, IEquatable<T>
    {
        public T Value { get; protected set; }

        public DomainEntityId()
        {
        }

        public DomainEntityId(T id)
        {
            Value = id;
        }
    }
}
