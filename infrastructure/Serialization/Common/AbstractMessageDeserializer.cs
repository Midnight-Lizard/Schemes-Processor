﻿using MediatR;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Common
{
    public abstract class AbstractMessageDeserializer<TMessage, TAggregateId>
        : IMessageDeserializer<TMessage>
        //where TMessage : BaseMessage
        where TMessage : DomainMessage<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        public virtual ITransportMessage<TMessage> DeserializeMessagePayload(
            string payload, JsonSerializerSettings serializerSettings, Guid correlationId, DateTime requestTimestamp)
        {
            var message = JsonConvert.DeserializeObject<TMessage>(payload, serializerSettings);
            AdvanceToTheLatestVersion(message);
            return new TransportMessage<TMessage, TAggregateId>(message, correlationId, requestTimestamp)
            {
                DeserializerType = this.GetType()
            };
        }

        public virtual void AdvanceToTheLatestVersion(TMessage message) { }
    }
}
