using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Processor.Application.DomainRequestHandlers
{
    public abstract class SchemeRequestHandler<TRequest> :
        AggregateRequestHandler<PublicScheme, TRequest, PublicSchemeId>
        where TRequest : SchemeDomainRequest
    {
        protected SchemeRequestHandler() : base()
        {
        }

        public override Task<PublicScheme> GetAggregate(PublicSchemeId id)
        {
            throw new NotImplementedException();
        }
    }
}
