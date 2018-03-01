using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Deserializers
{
    [Message(Version = "1.0")]
    public class SchemePublishedEventDeserializer_v1_0 : SchemePublishedEventDeserializer_v1_1
    {
        public override void AdvanceToTheLatestVersion(SchemePublishedEvent message)
        {
            // version 1.0 does not have scrollbar size and image hover options
            var cs = message.ColorScheme;
            cs.scrollbarSize = 10;//px
            cs.useImageHoverAnimation = cs.imageLightnessLimit > 80;

            base.AdvanceToTheLatestVersion(message);
        }
    }

    [Message(Version = "1.1")]
    public class SchemePublishedEventDeserializer_v1_1 : SchemePublishedEventDeserializer_v1_2
    {
        public override void AdvanceToTheLatestVersion(SchemePublishedEvent message)
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

    [Message(Version = "1.2")]
    public class SchemePublishedEventDeserializer_v1_2 : SchemePublishedEventDeserializer_Latest
    {
        public override void AdvanceToTheLatestVersion(SchemePublishedEvent message)
        {
            // version 1.2 does not have option to ignore color hues
            var cs = message.ColorScheme;
            cs.linkReplaceAllHues = true;

            base.AdvanceToTheLatestVersion(message);
        }
    }

    [Message(Version = "1.3")]
    public class SchemePublishedEventDeserializer_Latest : AbstractMessageDeserializer<SchemePublishedEvent>
    {
        public override ITransportMessage<SchemePublishedEvent> GreateTransportMessage(
            SchemePublishedEvent message, Guid correlationId, DateTime requestTimestamp)
        {
            return new TransportMessage<SchemePublishedEvent, PublicSchemeId>(message, correlationId, requestTimestamp);
        }
    }
}
