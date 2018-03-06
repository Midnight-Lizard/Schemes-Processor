using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Requests;
using MidnightLizard.Schemes.Processor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MidnightLizard.Schemes.Domain.PublisherAggregate;

namespace MidnightLizard.Schemes.Processor.Application.DomainRequestHandlers
{
    public class SchemePublishRequestHandler :
        DomainRequestHandler<PublicScheme, PublishSchemeRequest, PublicSchemeId>
    {
        public SchemePublishRequestHandler(
            IOptions<AggregatesConfig> cacheConfig,
            IMemoryCache memoryCache,
            IDomainEventDispatcher<PublicSchemeId> domainEventsDispatcher,
            IAggregateSnapshotAccessor<PublicScheme, PublicSchemeId> schemesSnapshot,
            IDomainEventStore<PublicSchemeId> eventsAccessor) :
            base(cacheConfig, memoryCache, domainEventsDispatcher, schemesSnapshot, eventsAccessor)
        {
        }

        protected override void HandleDomainRequest(PublicScheme aggregate, PublishSchemeRequest request, UserId userId, CancellationToken cancellationToken)
        {
            aggregate.Publish(new PublisherId(userId.Value), request.ColorScheme);
        }
    }
}
