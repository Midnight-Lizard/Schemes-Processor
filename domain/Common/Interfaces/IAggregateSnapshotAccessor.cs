using MidnightLizard.Schemes.Domain.Common.Results;
using MidnightLizard.Schemes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common.Interfaces
{
    public interface IAggregateSnapshotAccessor<TAggregate, TAggregateId>
        where TAggregateId : DomainEntityId
        where TAggregate : AggregateRoot<TAggregateId>
    {
        Task Save(AggregateSnapshot<TAggregate, TAggregateId> aggregate);

        Task<AggregateSnapshot<TAggregate, TAggregateId>> Read(TAggregateId id);
    }
}
