﻿using Autofac.Features.Indexed;
using MediatR;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Metadata;
using SemVer;
using MidnightLizard.Schemes.Infrastructure.Versioning;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Common
{
    public interface IMessageSerializer
    {
        string SerializeMessage(ITransportMessage<BaseMessage> transportMessage);
        string SerializeObject(object obj);
        MessageResult Deserialize(string message, DateTime messageTimestamp = default);
    }

    class MessageSerializer : IMessageSerializer
    {
        private readonly AppVersion version;
        private readonly IEnumerable<Meta<Lazy<IMessageDeserializer>>> deserializers;
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

        public MessageSerializer(
            AppVersion version,
            IEnumerable<Meta<Lazy<IMessageDeserializer>>> deserializers)
        {
            this.version = version;
            this.deserializers = deserializers;
        }

        private class Deserializable
        {
            public UserId UserId { get; set; }
            public Guid CorrelationId { get; set; }
            public string Type { get; set; }
            public string Version { get; set; }
            public DateTime? RequestTimestamp { get; set; }
            public DateTime? EventTimestamp { get; set; }
            public JRaw Payload { get; set; }
        }

        public virtual MessageResult Deserialize(string message, DateTime messageTimestamp = default)
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<Deserializable>(message, this.serializerSettings);
                var deserializer = this.deserializers.FirstOrDefault(x =>
                    x.Metadata[nameof(IMessageMetadata.Type)] as string == msg.Type &&
                    (x.Metadata[nameof(IMessageMetadata.VersionRange)] as Range).IsSatisfied(msg.Version));
                if (deserializer != null)
                {
                    return new MessageResult((deserializer.Value.Value as IMessageDeserializer<BaseMessage>)
                        .DeserializeMessagePayload(
                            msg.Payload.Value as string, this.serializerSettings,
                            msg.CorrelationId, msg.UserId,
                            msg.RequestTimestamp ?? messageTimestamp,
                            msg.EventTimestamp ?? messageTimestamp));
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
                Version = this.version.ToString(),
                transportMessage.RequestTimestamp,
                transportMessage.EventTimestamp,
                transportMessage.UserId,
                transportMessage.Payload
            }, this.serializerSettings);
        }

        public string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, this.serializerSettings);
        }
    }
}
