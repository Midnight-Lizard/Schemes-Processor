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
        string SerializeMessage(ITransportMessage<BaseMessage> transportMessage);
        string SerializeValue(object value);
        MessageResult Deserialize(string message, DateTime requestTimestamp = default);
    }

    class MessageSerializer : IMessageSerializer
    {
        private readonly IIndex<string, IMessageDeserializer> deserializers;
        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            //Formatting = Formatting.Indented,
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
            public Guid CorrelationId { get; set; }
            public string Type { get; set; }
            public string Version { get; set; }
            public DateTime? RequestTimestamp { get; set; }
            public JRaw Payload { get; set; }
        }

        public virtual MessageResult Deserialize(string message, DateTime requestTimestamp = default)
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<Deserializable>(message);
                string key = msg.Type + msg.Version;
                if (this.deserializers.TryGetValue(key, out var deserializer))
                {
                    return new MessageResult((deserializer as IMessageDeserializer<BaseMessage>)
                        .DeserializeMessagePayload(
                            msg.Payload.Value as string, this.serializerSettings,
                            msg.CorrelationId, msg.RequestTimestamp ?? requestTimestamp));
                }
                return new MessageResult($"Deserializer for message type {msg.Type} and version {msg.Version} is not found.");
            }
            catch (Exception ex)
            {
                return new MessageResult(ex);
            }
        }

        public virtual string SerializeMessage(ITransportMessage<BaseMessage> transportMessage)
        {
            return JsonConvert.SerializeObject(new
            {
                transportMessage.CorrelationId,
                Type = transportMessage.Payload.GetType().Name,
                Version = transportMessage.Payload.LatestVersion().ToString(),
                transportMessage.RequestTimestamp,
                transportMessage.Payload
            }, this.serializerSettings);
        }

        public string SerializeValue(object value)
        {
            return JsonConvert.SerializeObject(value, this.serializerSettings);
        }
    }
}
