using MidnightLizard.Schemes.Domain.PublisherAggregate;
using MidnightLizard.Schemes.Tests;
using System;
using System.Collections.Generic;
using NSubstitute;
using FluentAssertions;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public class PublicSchemeSpec
    {
        public class PiblishSpec : PublicSchemeSpec
        {
            [It(nameof(PublicScheme.Publish))]
            public void Should_not_be_New_after_successful_Publish()
            {
                var testScheme = new PublicScheme(true);
                testScheme.Publish(new PublisherId(), new ColorScheme());
                testScheme.IsNew().Should().BeFalse();
            }

        }
    }
}
