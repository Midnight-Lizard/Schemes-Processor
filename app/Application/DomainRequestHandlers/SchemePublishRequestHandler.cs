using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Processor.Application.DomainRequestHandlers
{
    public class SchemePublishRequestHandler : SchemeRequestHandler<SchemePublishRequest>
    {
        public async override Task<DomainRequestResult> Handle(SchemePublishRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
