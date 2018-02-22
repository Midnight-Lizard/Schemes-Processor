using Elasticsearch.Net;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Common
{
    public class DomainEventSerializer : IElasticsearchSerializer
    {
        private readonly IMessageSerializer messageSerializer;

        public DomainEventSerializer(IMessageSerializer messageSerializer)
        {
            this.messageSerializer = messageSerializer;
        }

        //public IPropertyMapping CreatePropertyMapping(MemberInfo memberInfo)
        //{
        //    return new PropertyMapping { Name = memberInfo.Name };
        //}

        public object Deserialize(Type type, Stream stream)
        {
            return DeserializeAsync(type, stream).GetAwaiter().GetResult();
        }

        public T Deserialize<T>(Stream stream)
        {
            return (T)this.Deserialize(typeof(T), stream);
        }

        public async Task<object> DeserializeAsync(Type type, Stream stream, CancellationToken cancellationToken = default)
        {
            if (typeof(ITransportMessage<BaseMessage>).IsAssignableFrom(type))
            {
                using (var reader = new StreamReader(stream))
                {
                    return messageSerializer.Deserialize(await reader.ReadToEndAsync(), DateTime.UtcNow);
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            return (T)await this.DeserializeAsync(typeof(T), stream, cancellationToken);
        }

        public void Serialize<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.Indented)
        {
            SerializeAsync<T>(data, stream, formatting).GetAwaiter().GetResult();
        }

        public void Serialize(object data, Stream writableStream, SerializationFormatting formatting = SerializationFormatting.Indented)
        {
            SerializeAsync<ITransportMessage<BaseMessage>>(data as ITransportMessage<BaseMessage>, writableStream, formatting).GetAwaiter().GetResult();
        }

        public async Task SerializeAsync<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.Indented, CancellationToken cancellationToken = default)
        {
            string json = "";
            switch (data)
            {
                case ITransportMessage<BaseMessage> message:
                    json = this.messageSerializer.SerializeMessage(message);
                    break;

                case var obj:
                    json = this.messageSerializer.SerializeValue(obj);
                    break;
            }
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(json);
            }
        }
    }
}
