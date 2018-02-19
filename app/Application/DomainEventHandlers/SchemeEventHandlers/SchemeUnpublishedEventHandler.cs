using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Processor.Application.DomainEventHandlers.SchemeEventHandlers
{
    public class SchemeUnpublishedEventHandler : DomainEventHandler<SchemeUnpublishedEvent, PublicSchemeId>
    {
        public SchemeUnpublishedEventHandler(IDomainEventStore<PublicSchemeId> domainEventAccessor)
            : base(domainEventAccessor)
        {
        }
    }
}
