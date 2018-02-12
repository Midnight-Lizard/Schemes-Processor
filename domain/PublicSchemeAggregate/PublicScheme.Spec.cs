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

            [It(nameof(PublicScheme.Publish))]
            public void Should_not_be_New_after_successful_Publish()
            {
                var testScheme = new PublicScheme(true);
                testScheme.Publish(new PublisherId(), this.correctColorScheme);
                testScheme.IsNew().Should().BeFalse();
            }

        }
    }
}
