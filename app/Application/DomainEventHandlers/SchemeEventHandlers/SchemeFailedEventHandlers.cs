using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.Common.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Processor.Application.DomainEventHandlers.SchemeEventHandlers
{
    public class ColorSchemeValidationFailedEventHandler
        : FailedDomainEventHandler<ColorSchemeValidationFailedEvent, PublicSchemeId>
    {
    }

    public class PublisherAccessDeniedEventHandler
        : FailedDomainEventHandler<PublisherAccessDeniedEvent, PublicSchemeId>
    {
    }

    public class PublisherIdValidationFailedEventHandler
        : FailedDomainEventHandler<PublisherIdValidationFailedEvent, PublicSchemeId>
    {
    }


}
