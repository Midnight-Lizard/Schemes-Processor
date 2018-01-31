﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public abstract class EntityId { }

    public class EntityId<T> : EntityId
        where T : IComparable, IComparable<T>, IEquatable<T>
    {
        public bool IsDefault
        {
            get
            {
                return object.Equals(default(T), Value);
            }
        }
        public T Value { get; protected set; }
    }
}
