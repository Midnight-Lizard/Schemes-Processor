using MediatR;
using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common.Messaging
{
    public class TransportMessage<TMessage, TAggregateId> : IRequest<DomainResult>
        where TAggregateId : DomainEntityId
        where TMessage : DomainMessage<TAggregateId>
    {
        public TMessage Message { get; }
        public Guid CorrelationId { get; }
        public DateTime RequestTimestamp { get; }

        public TransportMessage(TMessage message, Guid correlationId, DateTime requestTimestamp)
        {
            Message = message;
            CorrelationId = correlationId;
            RequestTimestamp = requestTimestamp;
        }
    }
}
