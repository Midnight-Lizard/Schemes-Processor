using MediatR;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Processor.Application.DomainRequestHandlers
{
    public abstract class AggregateRequestHandler<TAggregate, TRequest, TAggregateId> : IRequestHandler<TRequest, DomainRequestResult>
        where TRequest : DomainRequest<TAggregateId>
        where TAggregate : AggregateRoot<TAggregateId>
        where TAggregateId : EntityId
    {
        protected AggregateRequestHandler()
        {
        }

        public virtual async Task DispatchDomainEvents()
        {

        }

        public abstract Task<TAggregate> GetAggregate(TAggregateId id);
        public abstract Task<DomainRequestResult> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
