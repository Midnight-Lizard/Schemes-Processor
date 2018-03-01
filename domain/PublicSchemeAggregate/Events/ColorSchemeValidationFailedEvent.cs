using FluentValidation.Results;
using MidnightLizard.Commons.Domain.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events
{
    public class ColorSchemeValidationFailedEvent : ValidationFailedEvent<PublicSchemeId>
    {
        public override Version LatestVersion() => new Version(1, 0);

        protected ColorSchemeValidationFailedEvent() { }

        public ColorSchemeValidationFailedEvent(PublicSchemeId aggregateId, ValidationResult validationResult)
            : base(aggregateId, validationResult)
        {
        }
    }
}
