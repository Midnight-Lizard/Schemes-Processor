using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Processor.Application.DomainEventHandlers
{
    public class DomainEventHandlerSpec : DomainEventHandler<SchemePublishedEvent, PublicSchemeId>
    {
        public DomainEventHandlerSpec() : base(Substitute.For<IDomainEventAccessor<PublicSchemeId>>())
        {
        }
    }
}
