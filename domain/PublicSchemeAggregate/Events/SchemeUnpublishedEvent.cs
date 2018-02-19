using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events
{
    public class SchemeUnpublishedEvent : SchemeDomainEvent
    {
        public override Version LatestVersion() => new Version(1, 0);

        protected SchemeUnpublishedEvent() { }

        public SchemeUnpublishedEvent(PublicSchemeId aggregateId) : base(aggregateId)
        {
        }
    }
}
