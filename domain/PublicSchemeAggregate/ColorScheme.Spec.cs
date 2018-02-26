using MidnightLizard.Testing.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.TestHelper;
using Xunit;
using FluentValidation.Results;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public class ColorSchemeSpec
    {
        public static ColorScheme CorrectColorScheme
        {
            get => new ColorScheme
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
                scrollbarSize = 10,

                backgroundReplaceAllHues = false,
                borderReplaceAllHues = false,
                buttonReplaceAllHues = false,
                linkReplaceAllHues = true,
                textReplaceAllHues = false,

                buttonContrast = 50,
                buttonGrayHue = 40,
                buttonGraySaturation = 30,
                buttonLightnessLimit = 30,
                buttonSaturationLimit = 90,
            };
        }

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

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_colorSchemeName_is_too_long()
            {
                validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeName, new string('*', 51));
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_colorSchemeId_is_too_long()
            {
                validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeId, new string('*', 51));
            }

            [InlineData(360), InlineData(361), InlineData(400), InlineData(999)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_when_Hue_is_out_of_range(int value)
            {
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundGrayHue, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.borderGrayHue, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.buttonGrayHue, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.linkDefaultHue, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.linkVisitedHue, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarGrayHue, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.textGrayHue, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.textSelectionHue, value);
            }

            [InlineData(0), InlineData(180), InlineData(300), InlineData(259)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeed_when_Hue_is_in_0____359_range(int value)
            {
                validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundGrayHue, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.borderGrayHue, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.buttonGrayHue, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.linkDefaultHue, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.linkVisitedHue, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.scrollbarGrayHue, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.textGrayHue, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.textSelectionHue, value);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_when_any_setting_is_negative()
            {
                validator.ShouldHaveValidationErrorFor(cs => cs.blueFilter, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.scheduleStartHour, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.scheduleFinishHour, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundSaturationLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundContrast, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundLightnessLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundGraySaturation, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundGrayHue, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.textSaturationLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.textContrast, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.textLightnessLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.textGraySaturation, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.textGrayHue, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.textSelectionHue, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.linkSaturationLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.linkContrast, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.linkLightnessLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.linkDefaultSaturation, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.linkDefaultHue, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.linkVisitedHue, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.borderSaturationLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.borderContrast, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.borderLightnessLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.borderGraySaturation, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.borderGrayHue, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.imageLightnessLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.imageSaturationLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundImageLightnessLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundImageSaturationLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarSaturationLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarContrast, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarLightnessLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarGrayHue, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.buttonSaturationLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.buttonContrast, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.buttonLightnessLimit, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.buttonGraySaturation, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.buttonGrayHue, -1);
                validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarSize, -1);
            }

            [InlineData(101), InlineData(110), InlineData(200), InlineData(999)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_when_any_percentage_is_greater_than_100(int value)
            {
                validator.ShouldHaveValidationErrorFor(cs => cs.blueFilter, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundSaturationLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundContrast, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundLightnessLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundGraySaturation, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.textSaturationLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.textContrast, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.textLightnessLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.textGraySaturation, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.linkSaturationLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.linkContrast, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.linkLightnessLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.linkDefaultSaturation, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.borderSaturationLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.borderContrast, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.borderLightnessLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.borderGraySaturation, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.imageLightnessLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.imageSaturationLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundImageLightnessLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.backgroundImageSaturationLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarSaturationLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarContrast, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarLightnessLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.buttonSaturationLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.buttonContrast, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.buttonLightnessLimit, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.buttonGraySaturation, value);
            }

            [InlineData(0), InlineData(30), InlineData(60), InlineData(100)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeed_when_all_percentages_are_in_0____100_range(int value)
            {
                validator.ShouldNotHaveValidationErrorFor(cs => cs.blueFilter, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundSaturationLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundContrast, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundLightnessLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundGraySaturation, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.textSaturationLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.textContrast, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.textLightnessLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.textGraySaturation, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.linkSaturationLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.linkContrast, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.linkLightnessLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.linkDefaultSaturation, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.borderSaturationLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.borderContrast, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.borderLightnessLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.borderGraySaturation, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.imageLightnessLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.imageSaturationLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundImageLightnessLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.backgroundImageSaturationLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.scrollbarSaturationLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.scrollbarContrast, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.scrollbarLightnessLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.buttonSaturationLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.buttonContrast, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.buttonLightnessLimit, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.buttonGraySaturation, value);
            }

            [InlineData(25), InlineData(36), InlineData(48), InlineData(72)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_when_Hours_exceed_24(int value)
            {
                validator.ShouldHaveValidationErrorFor(cs => cs.scheduleStartHour, value);
                validator.ShouldHaveValidationErrorFor(cs => cs.scheduleFinishHour, value);
            }

            [InlineData(0), InlineData(8), InlineData(12), InlineData(15), InlineData(24)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeeed_when_Hours_are_in_0____24_range(int value)
            {
                validator.ShouldNotHaveValidationErrorFor(cs => cs.scheduleStartHour, value);
                validator.ShouldNotHaveValidationErrorFor(cs => cs.scheduleFinishHour, value);
            }

            [InlineData(51), InlineData(99), InlineData(100)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_fail_when_Scrollbar__Size_exceeds_50px(int px)
            {
                validator.ShouldHaveValidationErrorFor(cs => cs.scrollbarSize, px);
            }

            [InlineData(0), InlineData(5), InlineData(20), InlineData(50)]
            [Its(nameof(ColorSchemeValidator))]
            public void Should_succeeed_when_Scrollbar__Size_is_in_0____50px_range(int px)
            {
                validator.ShouldNotHaveValidationErrorFor(cs => cs.scrollbarSize, px);
            }

            [It(nameof(ColorSchemeValidator))]
            public void Should_fail_with_default_ColorScheme_object()
            {
                validator.ShouldHaveValidationErrorFor(cs => cs.colorSchemeName, new ColorScheme());
            }
        }
    }
}
