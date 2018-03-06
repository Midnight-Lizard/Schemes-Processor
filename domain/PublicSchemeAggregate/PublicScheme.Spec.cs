using MidnightLizard.Schemes.Domain.PublisherAggregate;
using MidnightLizard.Testing.Utilities;
using System;
using System.Collections.Generic;
using NSubstitute;
using FluentAssertions;
using System.Text;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public class PublicSchemeSpec
    {
        public class PiblishSpec : PublicSchemeSpec
        {
            private readonly ColorScheme incorrectColorScheme = new ColorScheme();
            private readonly PublicScheme newPublicScheme;
            private readonly PublicScheme existingPublicScheme;
            private readonly PublisherId testPublisherId = new PublisherId("test-user-id");
            private readonly PublicSchemeId testPublicSchemeId = new PublicSchemeId(Guid.NewGuid());

            public PiblishSpec()
            {
                this.existingPublicScheme = new PublicScheme(this.testPublicSchemeId);
                this.existingPublicScheme.Publish(this.testPublisherId, ColorSchemeSpec.CorrectColorScheme);
                this.existingPublicScheme.ReleaseEvents();

                this.newPublicScheme = new PublicScheme(this.testPublicSchemeId);
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_not_be_New_after_successful_Publish()
            {
                this.newPublicScheme.Publish(this.testPublisherId, ColorSchemeSpec.CorrectColorScheme);
                this.newPublicScheme.IsNew().Should().BeFalse();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_Release_ValidationFailedEvent_when_ColorScheme_is_invalid()
            {
                this.existingPublicScheme.Publish(this.testPublisherId, this.incorrectColorScheme);
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().HaveCount(1);
                events.Should().AllBeOfType<ColorSchemeValidationFailedEvent>();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_Release_AccessDenied_when_Publisher_is_different()
            {
                this.existingPublicScheme.Publish(new PublisherId("different-user-id"), this.incorrectColorScheme);
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().HaveCount(1);
                events.Should().AllBeOfType<PublisherAccessDeniedEvent>();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_Release_ValidationFailedEvent_when_PublisherId_is_invalid()
            {
                this.existingPublicScheme.Publish(new PublisherId(null), this.incorrectColorScheme);
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().HaveCount(1);
                events.Should().AllBeOfType<PublisherIdValidationFailedEvent>();
            }
        }
    }
}
