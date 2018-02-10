using MediatR;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.Common.Results;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization
{
    public interface IMessageDeserializer<TMessage> where TMessage : BaseMessage
    {
        TMessage DeserializeMessagePayload(string payload);
    }
}
