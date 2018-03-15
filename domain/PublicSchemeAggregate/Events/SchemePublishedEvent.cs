using MidnightLizard.Commons.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events
{
    public class SchemePublishedEvent : SchemeDomainEvent
    {
        public ColorScheme ColorScheme { get; private set; }

        protected SchemePublishedEvent() { }

        public SchemePublishedEvent(PublicSchemeId aggregateId, ColorScheme colorScheme)
            : base(aggregateId)
        {
            ColorScheme = colorScheme;
        }
    }
}
