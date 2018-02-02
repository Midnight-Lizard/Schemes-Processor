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
        protected readonly ISnapshot<PublicScheme, PublicSchemeId> schemesSnapshot;

        protected SchemeRequestHandler(
            IDomainEventsDispatcher<PublicSchemeId> eventsDispatcher,
            ISnapshot<PublicScheme, PublicSchemeId> schemesSnapshot,
            IDomainEventsAccessor<PublicSchemeId> eventsAccessor) :
            base(eventsDispatcher, eventsAccessor)
        {
            this.schemesSnapshot = schemesSnapshot;
        }

        protected override async Task<AggregateResult<PublicScheme>> GetAggregate(PublicSchemeId id)
        {
            var result = await this.schemesSnapshot.Read(id);
            if (!result.HasError)
            {

            }
            return result;
        }
    }
}
