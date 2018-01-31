using MidnightLizard.Schemes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public class ColorScheme : ValueObject, IColorScheme
    {
        public string colorSchemeId { get; set; }
        public string colorSchemeName { get; set; }
        public bool runOnThisSite { get; set; }
        public bool restoreColorsOnCopy { get; set; }
        public int blueFilter { get; set; }
        public bool useDefaultSchedule { get; set; }
        public int scheduleStartHour { get; set; }
        public int scheduleFinishHour { get; set; }
        public int backgroundSaturationLimit { get; set; }
        public int backgroundContrast { get; set; }
        public int backgroundLightnessLimit { get; set; }
        public int backgroundGraySaturation { get; set; }
        public int backgroundGrayHue { get; set; }
        public int textSaturationLimit { get; set; }
        public int textContrast { get; set; }
        public int textLightnessLimit { get; set; }
        public int textGraySaturation { get; set; }
        public int textGrayHue { get; set; }
        public int textSelectionHue { get; set; }
        public int linkSaturationLimit { get; set; }
        public int linkContrast { get; set; }
        public int linkLightnessLimit { get; set; }
        public int linkDefaultSaturation { get; set; }
        public int linkDefaultHue { get; set; }
        public int linkVisitedHue { get; set; }
        public int borderSaturationLimit { get; set; }
        public int borderContrast { get; set; }
        public int borderLightnessLimit { get; set; }
        public int borderGraySaturation { get; set; }
        public int borderGrayHue { get; set; }
        public int imageLightnessLimit { get; set; }
        public int imageSaturationLimit { get; set; }
        public int backgroundImageLightnessLimit { get; set; }
        public int backgroundImageSaturationLimit { get; set; }
        public int scrollbarSaturationLimit { get; set; }
        public int scrollbarContrast { get; set; }
        public int scrollbarLightnessLimit { get; set; }
        public int scrollbarGrayHue { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other != null && other is ColorScheme otherColorScheme)
            {
                return
                    otherColorScheme.backgroundContrast == this.backgroundContrast &&
                    otherColorScheme.backgroundGrayHue == this.backgroundGrayHue &&
                    otherColorScheme.backgroundGraySaturation == this.backgroundGraySaturation &&
                    otherColorScheme.backgroundImageLightnessLimit == this.backgroundImageLightnessLimit &&
                    otherColorScheme.backgroundImageSaturationLimit == this.backgroundImageSaturationLimit &&
                    otherColorScheme.backgroundLightnessLimit == this.backgroundLightnessLimit &&
                    otherColorScheme.backgroundSaturationLimit == this.backgroundSaturationLimit &&
                    otherColorScheme.blueFilter == this.blueFilter &&
                    otherColorScheme.borderContrast == this.borderContrast &&
                    otherColorScheme.borderGrayHue == this.borderGrayHue &&
                    otherColorScheme.borderGraySaturation == this.borderGraySaturation &&
                    otherColorScheme.borderLightnessLimit == this.borderLightnessLimit &&
                    otherColorScheme.borderSaturationLimit == this.borderSaturationLimit &&
                    otherColorScheme.colorSchemeId == this.colorSchemeId &&
                    otherColorScheme.colorSchemeName == this.colorSchemeName &&
                    otherColorScheme.imageLightnessLimit == this.imageLightnessLimit &&
                    otherColorScheme.imageSaturationLimit == this.imageSaturationLimit &&
                    otherColorScheme.linkContrast == this.linkContrast &&
                    otherColorScheme.linkDefaultHue == this.linkDefaultHue &&
                    otherColorScheme.linkDefaultSaturation == this.linkDefaultSaturation &&
                    otherColorScheme.linkLightnessLimit == this.linkLightnessLimit &&
                    otherColorScheme.linkSaturationLimit == this.linkSaturationLimit &&
                    otherColorScheme.linkVisitedHue == this.linkVisitedHue &&
                    otherColorScheme.restoreColorsOnCopy == this.restoreColorsOnCopy &&
                    otherColorScheme.runOnThisSite == this.runOnThisSite &&
                    otherColorScheme.scheduleFinishHour == this.scheduleFinishHour &&
                    otherColorScheme.scheduleStartHour == this.scheduleStartHour &&
                    otherColorScheme.scrollbarContrast == this.scrollbarContrast &&
                    otherColorScheme.scrollbarGrayHue == this.scrollbarGrayHue &&
                    otherColorScheme.scrollbarLightnessLimit == this.scrollbarLightnessLimit &&
                    otherColorScheme.scrollbarSaturationLimit == this.scrollbarSaturationLimit &&
                    otherColorScheme.textContrast == this.textContrast &&
                    otherColorScheme.textGrayHue == this.textGrayHue &&
                    otherColorScheme.textGraySaturation == this.textGraySaturation &&
                    otherColorScheme.textLightnessLimit == this.textLightnessLimit &&
                    otherColorScheme.textSaturationLimit == this.textSaturationLimit &&
                    otherColorScheme.textSelectionHue == this.textSelectionHue &&
                    otherColorScheme.useDefaultSchedule == this.useDefaultSchedule;
            }
            return false;
        }

        protected override IEnumerable<object> GetPropertyValues()
        {
            yield return colorSchemeId;
            yield return colorSchemeName;
            yield return runOnThisSite;
            yield return restoreColorsOnCopy;
            yield return blueFilter;
            yield return useDefaultSchedule;
            yield return scheduleStartHour;
            yield return scheduleFinishHour;
            yield return backgroundSaturationLimit;
            yield return backgroundContrast;
            yield return backgroundLightnessLimit;
            yield return backgroundGraySaturation;
            yield return backgroundGrayHue;
            yield return textSaturationLimit;
            yield return textContrast;
            yield return textLightnessLimit;
            yield return textGraySaturation;
            yield return textGrayHue;
            yield return textSelectionHue;
            yield return linkSaturationLimit;
            yield return linkContrast;
            yield return linkLightnessLimit;
            yield return linkDefaultSaturation;
            yield return linkDefaultHue;
            yield return linkVisitedHue;
            yield return borderSaturationLimit;
            yield return borderContrast;
            yield return borderLightnessLimit;
            yield return borderGraySaturation;
            yield return borderGrayHue;
            yield return imageLightnessLimit;
            yield return imageSaturationLimit;
            yield return backgroundImageLightnessLimit;
            yield return backgroundImageSaturationLimit;
            yield return scrollbarSaturationLimit;
            yield return scrollbarContrast;
            yield return scrollbarLightnessLimit;
            yield return scrollbarGrayHue;
        }
    }
}
