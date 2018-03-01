using MediatR;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Common
{
    public abstract class AbstractMessageDeserializer<TMessage> :
        IMessageDeserializer<TMessage> where TMessage : BaseMessage
    {
        public virtual ITransportMessage<TMessage> DeserializeMessagePayload(
            string payload, JsonSerializerSettings serializerSettings, Guid correlationId, DateTime requestTimestamp)
        {
            var message = JsonConvert.DeserializeObject<TMessage>(payload, serializerSettings);
            AdvanceToTheLatestVersion(message);
            var transMsg = GreateTransportMessage(message, correlationId, requestTimestamp);
            transMsg.DeserializerType = this.GetType();
            return transMsg;
        }

        public virtual void AdvanceToTheLatestVersion(TMessage message) { }

        public abstract ITransportMessage<TMessage> GreateTransportMessage(
            TMessage message, Guid correlationId, DateTime requestTimestamp);
    }
}
