using MidnightLizard.Schemes.Domain.PublisherAggregate;
using MidnightLizard.Schemes.Tests;
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
            private readonly ColorScheme incorrectColorScheme = new ColorScheme
            {
                colorSchemeId = "",
                colorSchemeName = new string('-', 51),
                runOnThisSite = true,
                restoreColorsOnCopy = false,
                blueFilter = 102,
                useDefaultSchedule = true,
                scheduleStartHour = 25,
                scheduleFinishHour = 32,
                backgroundSaturationLimit = 180,
                backgroundContrast = 150,
                backgroundLightnessLimit = 111,
                backgroundGraySaturation = 130,
                backgroundGrayHue = 436,
                textSaturationLimit = 190,
                textContrast = 160,
                textLightnessLimit = 180,
                textGraySaturation = 110,
                textGrayHue = 388,
                textSelectionHue = 436,
                linkSaturationLimit = 180,
                linkContrast = 150,
                linkLightnessLimit = 170,
                linkDefaultSaturation = -60,
                linkDefaultHue = 388,
                linkVisitedHue = 422,
                borderSaturationLimit = 180,
                borderContrast = 130,
                borderLightnessLimit = 150,
                borderGraySaturation = 120,
                borderGrayHue = 454,
                imageLightnessLimit = 180,
                imageSaturationLimit = 190,
                useImageHoverAnimation = false,
                backgroundImageLightnessLimit = 140,
                backgroundImageSaturationLimit = 180,
                scrollbarSaturationLimit = 210,
                scrollbarContrast = 220,
                scrollbarLightnessLimit = 140,
                scrollbarGrayHue = 445,
                scrollbarSize = 51
            };
            private readonly PublicScheme newPublicScheme;
            private readonly PublicScheme existingPublicScheme;
            private readonly PublisherId testPublisherId = new PublisherId(Guid.NewGuid());
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
                this.existingPublicScheme.Publish(new PublisherId(Guid.NewGuid()), this.incorrectColorScheme);
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().HaveCount(1);
                events.Should().AllBeOfType<PublisherAccessDeniedEvent>();
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_Release_ValidationFailedEvent_when_PublisherId_is_invalid()
            {
                this.existingPublicScheme.Publish(new PublisherId(), this.incorrectColorScheme);
                var events = this.existingPublicScheme.ReleaseEvents();
                events.Should().HaveCount(1);
                events.Should().AllBeOfType<PublisherIdValidationFailedEvent>();
            }
        }
    }
}
