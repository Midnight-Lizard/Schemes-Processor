using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class Event
    {
        public Guid Id { get; private set; }
        public string Type { get; protected set; }
        public Guid CorrelationId { get; protected set; }
        public Guid AggregateId { get; protected set; }
        public Version Version { get; protected set; }
        public DateTime Timestamp { get; protected set; }

        public Event()
        {
            Id = Guid.NewGuid();
        }
    }
}
