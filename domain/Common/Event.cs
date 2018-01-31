using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class DomainEvent
    {
        public Guid Id { get; private set; }
        public string Type { get; protected set; }
        public Guid CorrelationId { get; protected set; }
        public EntityId AggregateId { get; protected set; }
        public Version Version { get; protected set; }
        public int Offset { get; protected set; }
        public int RequestOffset { get; protected set; }
        public DateTime Timestamp { get; protected set; }

        public DomainEvent()
        {
            Id = Guid.NewGuid();
            Version = new Version(1, 0, 0);
        }

        public DomainEvent(Guid correlationId)
            : this()
        {
            CorrelationId = correlationId;
        }
    }
}
