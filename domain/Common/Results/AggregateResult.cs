using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common.Results
{
    public class AggregateResult<TAggregate, TAggregateId> : DomainResult
        where TAggregate : AggregateRoot<TAggregateId>
        where TAggregateId : DomainEntityId
    {
        public TAggregate Aggregate { get; }

        public AggregateResult() { }

        public AggregateResult(TAggregate aggregate)
        {
            Aggregate = aggregate;
        }

        public AggregateResult(string errorMessage) : base(errorMessage)
        {

        }

        public AggregateResult(Exception ex) : base(ex)
        {
        }

        public AggregateResult(bool hasError, Exception ex, string errorMessage) : base(hasError, ex, errorMessage)
        {
        }
    }
}
