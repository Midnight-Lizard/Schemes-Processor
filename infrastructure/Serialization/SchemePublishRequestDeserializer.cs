using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization
{
    [MessageVersion("1.0")]
    public class SchemePublishRequestDeserializer_v1_0 : SchemePublishRequestDeserializer_v1_1
    {
        public override SchemePublishRequest DeserializeMessagePayload(string payload)
        {
            var r = base.DeserializeMessagePayload(payload);

            // version 1.0 does not contain button component
            r.ColorScheme.buttonSaturationLimit = (int)Math.Round(Math.Min(r.ColorScheme.backgroundSaturationLimit * 1.1, 100));
            r.ColorScheme.buttonContrast = r.ColorScheme.backgroundContrast;
            r.ColorScheme.buttonLightnessLimit = (int)Math.Round(r.ColorScheme.backgroundLightnessLimit * 0.8);
            r.ColorScheme.buttonGraySaturation = (int)Math.Round(Math.Min(r.ColorScheme.backgroundGraySaturation * 1.1, 100));
            r.ColorScheme.buttonGrayHue = r.ColorScheme.borderGrayHue;

            return r;
        }
    }

    [MessageVersion("1.1")]
    public class SchemePublishRequestDeserializer_v1_1 : SchemePublishRequestDeserializer_v1_2
    {
        public override SchemePublishRequest DeserializeMessagePayload(string payload)
        {
            var r = base.DeserializeMessagePayload(payload);

            // version 1.1 does not contain option to ignore color hues
            r.ColorScheme.linkReplaceAllHues = true;

            return r;
        }
    }

    [MessageVersion("1.2")]
    public class SchemePublishRequestDeserializer_v1_2 : BaseMessageDeserializer<SchemePublishRequest>
    {
    }
}
