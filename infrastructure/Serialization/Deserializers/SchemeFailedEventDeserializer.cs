﻿using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Deserializers
{
    public abstract class SchemeFailedEventDeserializer<TMessage> :
        AbstractMessageDeserializer<TMessage, PublicSchemeId>
        where TMessage : DomainMessage<PublicSchemeId>
    {
        public override void StartAdvancingToTheLatestVersion(TMessage message)
        {
        }
    }

    [Message(Version = "*")]
    public class ColorSchemeValidationFailedEventDeserializer :
        SchemeFailedEventDeserializer<ColorSchemeValidationFailedEvent>
    {
    }

    [Message(Version = "*")]
    public class SchemeAccessDeniedEventDeserializer :
        SchemeFailedEventDeserializer<SchemeAccessDeniedEvent>
    {
    }

    [Message(Version = "*")]
    public class PublisherIdValidationFailedEventDeserializer :
        SchemeFailedEventDeserializer<PublisherIdValidationFailedEvent>
    {
    }
}
