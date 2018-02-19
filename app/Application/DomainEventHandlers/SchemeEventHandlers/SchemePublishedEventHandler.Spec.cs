using FluentAssertions;
using MediatR;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Schemes.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ITransEvent = MidnightLizard.Schemes.Domain.Common.Messaging.ITransportMessage<MidnightLizard.Schemes.Domain.Common.Messaging.BaseMessage>;
using TransEvent = MidnightLizard.Schemes.Domain.Common.Messaging.TransportMessage<MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events.SchemePublishedEvent, MidnightLizard.Schemes.Domain.PublicSchemeAggregate.PublicSchemeId>;

namespace MidnightLizard.Schemes.Processor.Application.DomainEventHandlers.SchemeEventHandlers
{
    public class SchemePublishedEventHandlerSpec : SchemePublishedEventHandler
    {
        private static int handle_CallCount;
        private IMediator mediator;
        private readonly ITransEvent testTransEvent = new TransEvent(new SchemePublishedEvent(null, null, null), Guid.NewGuid(), DateTime.UtcNow);

        public SchemePublishedEventHandlerSpec() : base(Substitute.For<IDomainEventStore<PublicSchemeId>>())
        {
            this.mediator = StartupStub.Resolve<IMediator>();
        }

        public override async Task<DomainResult> Handle(TransEvent request, CancellationToken cancellationToken)
        {
            SchemePublishedEventHandlerSpec.handle_CallCount++;
            return DomainResult.Ok;
        }

        [It(nameof(MediatR))]
        public async Task Should_handle_Event()
        {
            SchemePublishedEventHandlerSpec.handle_CallCount = 0;

            var result = await this.mediator.Send(this.testTransEvent);

            SchemePublishedEventHandlerSpec.handle_CallCount.Should().Be(1);
        }
    }
}
