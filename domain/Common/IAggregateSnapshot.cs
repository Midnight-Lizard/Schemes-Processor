using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common
{
    public interface IAggregateSnapshot<TAggregate, TAggregateId>
        where TAggregateId : DomainEntityId
        where TAggregate : AggregateRoot<TAggregateId>
    {
        Task<DomainResult> Save(TAggregate scheme);

        Task<AggregateResult<TAggregate>> Read(TAggregateId id);
    }
}
