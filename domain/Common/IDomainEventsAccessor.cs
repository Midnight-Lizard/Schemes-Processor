using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common
{
    /// <summary>
    /// Reads and writes domain events into events store
    /// </summary>
    /// <typeparam name="TAggregateId">Type of Aggregate ID events of wich this accessor processing</typeparam>
    public interface IDomainEventsAccessor<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        Task<DomainEventsResult<TAggregateId>> Read(TAggregateId id, int offset);
        Task<DomainResult> Write(DomainEvent<TAggregateId> @event);
    }
}
