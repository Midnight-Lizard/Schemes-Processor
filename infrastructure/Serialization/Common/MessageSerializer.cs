using Autofac.Features.Indexed;
using MediatR;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.Common.Results;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Common
{
    public interface IMessageSerializer
    {
        string Serialize<TMessage>(ITransportMessage<TMessage> transportMessage) where TMessage : BaseMessage;
        MessageResult Deserialize(string message);
    }

    class MessageSerializer : IMessageSerializer
    {
        private readonly IIndex<string, IMessageDeserializer> deserializers;
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = MessageContractResolver.Default,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Converters = new[] {
                    new DomainEntityIdConverter()
                }
        };

        public MessageSerializer(IIndex<string, IMessageDeserializer> deserializers)
        {
            this.deserializers = deserializers;
        }

        private class Deserializable
        {
            public string Type { get; set; }
            public string Version { get; set; }
            public JRaw Payload { get; set; }
            public DateTime RequestTimestamp { get; set; }
            public Guid CorrelationId { get; set; }
        }

        public virtual MessageResult Deserialize(string message)
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<Deserializable>(message);
                string key = msg.Type + msg.Version;
                if (this.deserializers.TryGetValue(key, out var deserializer))
                {
                    return new MessageResult((deserializer as IMessageDeserializer<BaseMessage>)
                        .DeserializeMessagePayload(
                            msg.Payload.Value as string, serializerSettings,
                            msg.CorrelationId, msg.RequestTimestamp));
                }
                return new MessageResult($"Deserializer for message type {msg.Type} and version {msg.Version} is not found.");
            }
            catch (Exception ex)
            {
                return new MessageResult(ex);
            }
        }

        public virtual string Serialize<TMessage>(ITransportMessage<TMessage> transportMessage)
            where TMessage : BaseMessage
        {
            return JsonConvert.SerializeObject(new
            {
                transportMessage.CorrelationId,
                Type = transportMessage.Payload.GetType().Name,
                Version = transportMessage.Payload.LatestVersion().ToString(),
                transportMessage.RequestTimestamp,
                transportMessage.Payload
            }, serializerSettings);
        }
    }
}
