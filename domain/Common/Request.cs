using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MidnightLizard.Schemes.Domain.Common
{
    public abstract class DomainRequest<TAggregateId> : IRequest<DomainResult>
        where TAggregateId : EntityId
    {
        public virtual string Type { get; protected set; }
        public Guid Id { get; protected set; }
        public Guid CorrelationId { get; protected set; }
        public TAggregateId AggregateId { get; protected set; }
        public Version Version { get; protected set; }
        public int Offset { get; protected set; }
        public DateTime Timestamp { get; protected set; }

        public DomainRequest()
        {
            Id = CorrelationId = Guid.NewGuid();
            Version = new Version(1, 0, 0);
            var reg = new Regex("");
            Type = string.Join("-", Regex.Split(this.GetType().Name, @"((?:[A-Z])+[^A-Z]*)")).ToLower();
        }
    }
}
