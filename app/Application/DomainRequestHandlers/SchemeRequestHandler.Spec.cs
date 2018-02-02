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
    public class SchemeRequestHandlerSpec : SchemeRequestHandler<SchemeDomainRequest>,
        IClassFixture<Stub<IDomainEventsDispatcher<PublicSchemeId>>>,
        IClassFixture<Stub<ISnapshot<PublicScheme, PublicSchemeId>>>,
        IClassFixture<Stub<IDomainEventsAccessor<PublicSchemeId>>>
    {
        protected readonly Stub<ISnapshot<PublicScheme, PublicSchemeId>> snapshotStub;

        public SchemeRequestHandlerSpec(
            Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub,
            Stub<ISnapshot<PublicScheme, PublicSchemeId>> snapshotStub,
            Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessor) :
            base(dispatcherStub.Object, snapshotStub.Object, eventsAccessor.Object)
        {
            this.snapshotStub = snapshotStub;
            this.snapshotStub.Reset();
            this.snapshotStub
                .Setup(s => s.Read(It.IsAny<PublicSchemeId>()))
                .ReturnsAsync(new AggregateResult<PublicScheme>(new PublicScheme()));
        }

        public override Task<DomainResult> Handle(SchemeDomainRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [It(nameof(Should_restore_SchemesAggregate_from_Snapshot_and_apply_new_Events))]
        public async Task Should_restore_SchemesAggregate_from_Snapshot_and_apply_new_Events()
        {
            var result = await this.GetAggregate(new PublicSchemeId());
            this.snapshotStub.Verify(s => s.Read(It.IsAny<PublicSchemeId>()), Times.Once);
            Assert.False(result.HasError);
        }
    }
}
