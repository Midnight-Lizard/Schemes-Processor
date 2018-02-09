using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common.Interfaces
{
    /// <summary>
    /// Reads and writes domain events into events store
    /// </summary>
    /// <typeparam name="TAggregateId">Type of Aggregate ID events of wich this accessor processing</typeparam>
    public interface IDomainEventsAccessor<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        Task<DomainEventsResult<TAggregateId>> Read(IAggregateOffset<TAggregateId> aggregateOffset);
        Task<DomainResult> Write(DomainEvent<TAggregateId> @event);
    }
}
