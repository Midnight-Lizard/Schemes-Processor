using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Deserializers
{
    [MessageVersion("1.0")]
    public class SchemePublishRequestDeserializer_v1_0 : SchemePublishRequestDeserializer_v1_1
    {
        public override void AdvanceToTheLatestVersion(SchemePublishRequest message)
        {
            // version 1.0 does not have scrollbar size and image hover options
            var cs = message.ColorScheme;
            cs.scrollbarSize = 10;//px
            cs.useImageHoverAnimation = cs.imageLightnessLimit > 80;

            base.AdvanceToTheLatestVersion(message);
        }
    }

    [MessageVersion("1.1")]
    public class SchemePublishRequestDeserializer_v1_1 : SchemePublishRequestDeserializer_v1_2
    {
        public override void AdvanceToTheLatestVersion(SchemePublishRequest message)
        {
            // version 1.1 does not have button component
            var cs = message.ColorScheme;
            cs.buttonSaturationLimit = (int)Math.Round(Math.Min(cs.backgroundSaturationLimit * 1.1, 100));
            cs.buttonContrast = cs.backgroundContrast;
            cs.buttonLightnessLimit = (int)Math.Round(cs.backgroundLightnessLimit * 0.8);
            cs.buttonGraySaturation = (int)Math.Round(Math.Min(cs.backgroundGraySaturation * 1.1, 100));
            cs.buttonGrayHue = cs.borderGrayHue;

            base.AdvanceToTheLatestVersion(message);
        }
    }

    [MessageVersion("1.2")]
    public class SchemePublishRequestDeserializer_v1_2 : SchemePublishRequestDeserializer_v1_3
    {
        public override void AdvanceToTheLatestVersion(SchemePublishRequest message)
        {
            // version 1.2 does not have option to ignore color hues
            var cs = message.ColorScheme;
            cs.linkReplaceAllHues = true;

            base.AdvanceToTheLatestVersion(message);
        }
    }

    [MessageVersion("1.3")]
    public class SchemePublishRequestDeserializer_v1_3 : AbstractMessageDeserializer<SchemePublishRequest>
    {
        public override ITransportMessage<SchemePublishRequest> GreateTransportMessage(
               SchemePublishRequest message, Guid correlationId, DateTime requestTimestamp)
        {
            return new TransportMessage<SchemePublishRequest, PublicSchemeId>(message, correlationId, requestTimestamp);
        }
    }
}
