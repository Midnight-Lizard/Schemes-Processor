using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Tests;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MidnightLizard.Schemes.Processor.Application.DomainRequestHandlers
{
    public class AggregateRequestHandlerSpec :
        AggregateRequestHandler<PublicScheme, DomainRequest<PublicSchemeId>, PublicSchemeId>,
        IClassFixture<Stub<IDomainEventsDispatcher<PublicSchemeId>>>,
        IClassFixture<Stub<IDomainEventsAccessor<PublicSchemeId>>>
    {
        protected readonly Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub;

        public AggregateRequestHandlerSpec(
            Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub,
           Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessor) :
            base(dispatcherStub.Object, eventsAccessor.Object)
        {
            this.dispatcherStub = dispatcherStub;
            this.dispatcherStub.Reset();
            this.dispatcherStub
                .Setup(d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()))
                .ReturnsAsync(DomainResult.Ok);
        }

        protected override Task<AggregateResult<PublicScheme>> GetAggregate(PublicSchemeId id)
        {
            throw new NotImplementedException();
        }

        public override Task<DomainResult> Handle(DomainRequest<PublicSchemeId> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [It(nameof(Should_dispatch_DomainEvents_from_supplied_Aggregate))]
        public async Task Should_dispatch_DomainEvents_from_supplied_Aggregate()
        {
            var scheme = new PublicScheme();
            scheme.Events.AddRange(new[] {
                new SchemePublishedEvent(),
                new SchemePublishedEvent()
            });
            var results = await this.DispatchDomainEvents(scheme);
            Assert.All(results, result => Assert.False(result.HasError));
            Assert.Equal(scheme.Events.Count, results.Count);
            dispatcherStub.Verify(
                d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()),
                Times.Exactly(scheme.Events.Count));
        }
    }
}
