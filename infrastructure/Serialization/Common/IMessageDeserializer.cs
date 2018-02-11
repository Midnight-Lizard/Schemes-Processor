using MediatR;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.Common.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Common
{
    public interface IMessageDeserializer
    {

    }

    public interface IMessageDeserializer<out TMessage> : IMessageDeserializer where TMessage : BaseMessage
    {
        ITransportMessage<TMessage> DeserializeMessagePayload(string payload, JsonSerializerSettings serializerSettings, Guid correlationId, DateTime requestTimestamp);
    }
}
