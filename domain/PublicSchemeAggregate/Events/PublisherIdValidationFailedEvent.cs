using FluentValidation.Results;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events
{
    public class PublisherIdValidationFailedEvent : ValidationFailedEvent<PublicSchemeId>
    {
        public override Version LatestVersion() => new Version(1, 0);

        protected PublisherIdValidationFailedEvent() { }

        public PublisherIdValidationFailedEvent(PublicSchemeId aggregateId, ValidationResult validationResult)
            : base(aggregateId, validationResult)
        {
        }
    }
}
