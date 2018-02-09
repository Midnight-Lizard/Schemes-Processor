using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public interface IColorScheme
    {
        string colorSchemeId { get; set; }
        string colorSchemeName { get; set; }
        bool runOnThisSite { get; set; }
        bool restoreColorsOnCopy { get; set; }
        int blueFilter { get; set; }
        bool useDefaultSchedule { get; set; }
        int scheduleStartHour { get; set; }
        int scheduleFinishHour { get; set; }
        int backgroundSaturationLimit { get; set; }
        int backgroundContrast { get; set; }
        int backgroundLightnessLimit { get; set; }
        int backgroundGraySaturation { get; set; }
        int backgroundGrayHue { get; set; }
        int textSaturationLimit { get; set; }
        int textContrast { get; set; }
        int textLightnessLimit { get; set; }
        int textGraySaturation { get; set; }
        int textGrayHue { get; set; }
        int textSelectionHue { get; set; }
        int linkSaturationLimit { get; set; }
        int linkContrast { get; set; }
        int linkLightnessLimit { get; set; }
        int linkDefaultSaturation { get; set; }
        int linkDefaultHue { get; set; }
        int linkVisitedHue { get; set; }
        int borderSaturationLimit { get; set; }
        int borderContrast { get; set; }
        int borderLightnessLimit { get; set; }
        int borderGraySaturation { get; set; }
        int borderGrayHue { get; set; }
        int imageLightnessLimit { get; set; }
        int imageSaturationLimit { get; set; }
        int backgroundImageLightnessLimit { get; set; }
        int backgroundImageSaturationLimit { get; set; }
        int scrollbarSaturationLimit { get; set; }
        int scrollbarContrast { get; set; }
        int scrollbarLightnessLimit { get; set; }
        int scrollbarGrayHue { get; set; }


        int buttonBackgroundSaturationLimit { get; set; }
        int buttonBackgroundContrast { get; set; }
        int buttonBackgroundLightnessLimit { get; set; }
        int buttonBackgroundGraySaturation { get; set; }
        int buttonBackgroundGrayHue { get; set; }
    }
}
