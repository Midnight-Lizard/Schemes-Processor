using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public abstract class SchemeDomainEvent : DomainEvent<PublicSchemeId>
    {
        protected SchemeDomainEvent() : base() { }

        public SchemeDomainEvent(PublicSchemeId aggregateId) : base(aggregateId)
        {
        }
    }
}
