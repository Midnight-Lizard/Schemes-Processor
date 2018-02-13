using MediatR;
using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common.Messaging
{
    public interface ITransportMessage<out TMessage>
        where TMessage : BaseMessage
    {
        TMessage Payload { get; }
        Guid CorrelationId { get; }
        DateTime RequestTimestamp { get; }
        Type DeserializerType { get; set; }
    }

    public class TransportMessage<TMessage, TAggregateId> : IRequest<DomainResult>, ITransportMessage<TMessage>
        where TAggregateId : DomainEntityId
        where TMessage : DomainMessage<TAggregateId>
    {
        public TMessage Payload { get; }
        public Guid CorrelationId { get; }
        public DateTime RequestTimestamp { get; }
        public Type DeserializerType { get; set; }

        public TransportMessage(TMessage message, Guid correlationId, DateTime requestTimestamp)
        {
            Payload = message;
            CorrelationId = correlationId;
            RequestTimestamp = requestTimestamp;
        }
    }
}
