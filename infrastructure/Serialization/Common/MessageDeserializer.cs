using Autofac.Features.Indexed;
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
    public class MessageDeserializer
    {
        private readonly IIndex<string, IMessageDeserializer<BaseMessage>> deserializers;

        public MessageDeserializer(IIndex<string, IMessageDeserializer<BaseMessage>> deserializers)
        {
            this.deserializers = deserializers;
        }

        public virtual MessageResult Deserialize(string message)
        {
            var msg = JsonConvert.DeserializeObject<(string Type, string Version)>(message);
            var key = msg.Type + msg.Version;
            if (this.deserializers.TryGetValue(key, out var deserializer))
            {
                return new MessageResult(deserializer.ParseMessage(message));

            }
            return new MessageResult($"Deserializer for message type {msg.Type} and version {msg.Version} is not found.");
        }
    }
}
