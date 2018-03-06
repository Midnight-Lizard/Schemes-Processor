using FluentAssertions;
using MediatR;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Testing.Utilities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ITransEvent = MidnightLizard.Commons.Domain.Messaging.ITransportMessage<MidnightLizard.Commons.Domain.Messaging.BaseMessage>;
using TransEvent = MidnightLizard.Commons.Domain.Messaging.TransportMessage<MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events.SchemeUnpublishedEvent, MidnightLizard.Schemes.Domain.PublicSchemeAggregate.PublicSchemeId>;

namespace MidnightLizard.Schemes.Processor.Application.DomainEventHandlers.SchemeEventHandlers
{
    public class SchemeUnpublishedEventHandlerSpec : SchemeUnpublishedEventHandler
    {
        private static int handle_CallCount;
        private IMediator mediator;
        private readonly ITransEvent testTransEvent = new TransEvent(new SchemeUnpublishedEvent(null), Guid.NewGuid(), DateTime.UtcNow, new UserId("test-user-id"));

        public SchemeUnpublishedEventHandlerSpec() : base(Substitute.For<IDomainEventStore<PublicSchemeId>>())
        {
            this.mediator = StartupStub.Resolve<IMediator>();
        }

        public override Task<DomainResult> Handle(TransEvent @event, CancellationToken cancellationToken)
        {
            SchemeUnpublishedEventHandlerSpec.handle_CallCount++;
            return Task.FromResult(DomainResult.Ok);
        }

        [It(nameof(MediatR))]
        public async Task Should_handle_Event()
        {
            SchemeUnpublishedEventHandlerSpec.handle_CallCount = 0;

            var result = await this.mediator.Send(this.testTransEvent);

            SchemeUnpublishedEventHandlerSpec.handle_CallCount.Should().Be(1);
        }
    }
}
