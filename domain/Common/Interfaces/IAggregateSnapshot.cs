using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common.Interfaces
{
    public interface IAggregateSnapshot<TAggregate, TAggregateId>
        where TAggregateId : DomainEntityId
        where TAggregate : AggregateRoot<TAggregateId>
    {
        Task<DomainResult> Save(TAggregate scheme);

        Task<AggregateResult<TAggregate, TAggregateId>> Read(TAggregateId id);
    }
}
