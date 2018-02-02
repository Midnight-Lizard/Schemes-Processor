using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class DomainEvent<TAggregateId> : IRequest<DomainResult>
        where TAggregateId : EntityId
    {
        public string Type { get; protected set; }
        public Guid Id { get; private set; }
        public Guid CorrelationId { get; protected set; }
        public TAggregateId AggregateId { get; protected set; }
        public Version Version { get; protected set; }
        public int Offset { get; protected set; }
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
    }
}
