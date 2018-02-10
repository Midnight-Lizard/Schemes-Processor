using MediatR;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.Common.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization
{
    public abstract class BaseMessageDeserializer<TMessage> :
        IMessageDeserializer<TMessage> where TMessage : BaseMessage
    {
        public virtual TMessage DeserializeMessagePayload(string payload)
        {
            return JsonConvert.DeserializeObject<TMessage>(payload, new JsonSerializerSettings
            {
                ContractResolver = MessageContractResolver.Default,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            });
        }
    }
}
