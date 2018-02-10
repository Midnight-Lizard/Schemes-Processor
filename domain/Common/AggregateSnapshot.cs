using MidnightLizard.Schemes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class AggregateSnapshot<TAggregate, TAggregateId>
        where TAggregate : AggregateRoot<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        public TAggregate Aggregate { get; }
        public DateTime RequestTimestamp { get; set; }

        public AggregateSnapshot(TAggregate aggregate, DateTime requestTimestamp)
        {
            Aggregate = aggregate;
            RequestTimestamp = requestTimestamp;
        }
    }
}
