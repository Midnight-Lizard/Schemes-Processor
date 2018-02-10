using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public class ColorSchemeValidator : AbstractValidator<ColorScheme>
    {
        public ColorSchemeValidator()
        {
            RuleFor(cs => cs.scheduleFinishHour).InclusiveBetween(0, 24);
            RuleFor(cs => cs.scheduleStartHour).InclusiveBetween(0, 24);
            RuleFor(cs => cs.colorSchemeId).NotEmpty().Length(1, 50);
            RuleFor(cs => cs.colorSchemeName).NotEmpty().Length(1, 50);
        }
    }
}
