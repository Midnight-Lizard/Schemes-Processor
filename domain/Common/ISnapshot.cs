using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common
{
    public interface ISnapshot<TAggregate, TAggregateId>
        where TAggregateId : EntityId
        where TAggregate : AggregateRoot<TAggregateId>
    {
        Task<DomainResult> Save(TAggregate scheme);

        Task<AggregateResult<TAggregate>> Read(TAggregateId id);
    }
}
