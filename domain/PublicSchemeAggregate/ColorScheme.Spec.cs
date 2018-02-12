using MidnightLizard.Schemes.Tests;
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.TestHelper;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public class ColorSchemeSpec
    {
        public class ValidatorSpec
        {
            private readonly ColorSchemeValidator validator = new ColorSchemeValidator();

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_colorSchemeName_is_null()
            {
                validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeName, null as string);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_colorSchemeId_is_null()
            {
                validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeId, null as string);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_colorSchemeName_is_empty()
            {
                validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeName, string.Empty);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_colorSchemeId_is_empty()
            {
                validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeId, string.Empty);
            }
        }
    }
}
