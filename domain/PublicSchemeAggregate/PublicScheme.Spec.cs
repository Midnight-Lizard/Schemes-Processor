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
            private readonly ColorScheme correctColorScheme = new ColorScheme
            {
                colorSchemeId = "almondRipe",
                colorSchemeName = "Almond Ripe",
                runOnThisSite = true,
                restoreColorsOnCopy = false,
                blueFilter = 5,
                useDefaultSchedule = true,
                scheduleStartHour = 0,
                scheduleFinishHour = 24,
                backgroundSaturationLimit = 80,
                backgroundContrast = 50,
                backgroundLightnessLimit = 11,
                backgroundGraySaturation = 30,
                backgroundGrayHue = 36,
                textSaturationLimit = 90,
                textContrast = 60,
                textLightnessLimit = 80,
                textGraySaturation = 10,
                textGrayHue = 88,
                textSelectionHue = 36,
                linkSaturationLimit = 80,
                linkContrast = 50,
                linkLightnessLimit = 70,
                linkDefaultSaturation = 60,
                linkDefaultHue = 88,
                linkVisitedHue = 122,
                borderSaturationLimit = 80,
                borderContrast = 30,
                borderLightnessLimit = 50,
                borderGraySaturation = 20,
                borderGrayHue = 54,
                imageLightnessLimit = 80,
                imageSaturationLimit = 90,
                useImageHoverAnimation = false,
                backgroundImageLightnessLimit = 40,
                backgroundImageSaturationLimit = 80,
                scrollbarSaturationLimit = 20,
                scrollbarContrast = 0,
                scrollbarLightnessLimit = 40,
                scrollbarGrayHue = 45,
                scrollbarSize = 10
            };
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
                this.existingPublicScheme.Publish(this.testPublisherId, this.correctColorScheme);
                this.existingPublicScheme.ReleaseEvents();

                this.newPublicScheme = new PublicScheme(this.testPublicSchemeId);
            }

            [It(nameof(PublicScheme.Publish))]
            public void Should_not_be_New_after_successful_Publish()
            {
                this.newPublicScheme.Publish(this.testPublisherId, this.correctColorScheme);
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
