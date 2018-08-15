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
    [Message(Version = "*")]
    public class SchemeUnpublishedEventDeserializer : AbstractMessageDeserializer<SchemeUnpublishedEvent, PublicSchemeId>
    {
        public override void StartAdvancingToTheLatestVersion(SchemeUnpublishedEvent message)
        {
        }
    }
}
