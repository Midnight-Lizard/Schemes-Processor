using MediatR;
using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common.Messaging
{
    public abstract class BaseMessage : IEquatable<BaseMessage>
    {
        public Guid Id { get; protected set; }
        public abstract Version LatestVersion();

        public override bool Equals(object obj)
        {
            return obj != null &&
               obj is BaseMessage other &&
               other.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public bool Equals(BaseMessage other)
        {
            return other != null && other.Id == this.Id;
        }
    }
}
