using MediatR;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MidnightLizard.Schemes.Domain.Common
{
    public abstract class DomainEvent<TAggregateId> : IRequest<DomainResult>, IEquatable<DomainEvent<TAggregateId>>
        where TAggregateId : DomainEntityId
    {
        public string Type { get; protected set; }
        public Guid Id { get; private set; }
        public Guid CorrelationId { get; protected set; }
        public TAggregateId AggregateId { get; protected set; }
        public Version Version { get; protected set; }
        public int EventOffset { get; protected set; }
        public int RequestOffset { get; protected set; }
        public DateTime Timestamp { get; protected set; }

        public DomainEvent()
        {
            Id = Guid.NewGuid();
            Version = new Version(1, 0, 0);
            Type = string.Join("-", Regex.Split(this.GetType().Name, @"((?:[A-Z])+[^A-Z]*)")).ToLower();
        }

        public DomainEvent(Guid correlationId)
            : this()
        {
            CorrelationId = correlationId;
        }

        public override bool Equals(object obj)
        {
            return obj != null &&
               obj is DomainEvent<TAggregateId> other &&
               other.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public bool Equals(DomainEvent<TAggregateId> other)
        {
            return other != null && other.Id == this.Id;
        }
    }
}
