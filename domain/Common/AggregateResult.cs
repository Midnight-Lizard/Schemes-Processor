using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class AggregateResult<TAggregate> : DomainResult
    {
        public TAggregate Aggregate { get; }

        public AggregateResult() { }

        public AggregateResult(TAggregate aggregate)
        {
            Aggregate = aggregate;
        }

        public AggregateResult(Exception ex) : base(ex)
        {
        }

        public AggregateResult(bool hasError, Exception ex, string errorMessage) : base(hasError, ex, errorMessage)
        {
        }
    }
}
