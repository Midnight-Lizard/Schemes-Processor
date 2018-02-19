using FluentAssertions;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Messaging;
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

namespace MidnightLizard.Schemes.Processor.Application.DomainEventHandlers
{
    public class DomainEventHandlerSpec : DomainEventHandler<SchemePublishedEvent, PublicSchemeId>
    {
        private readonly TransportMessage<SchemePublishedEvent, PublicSchemeId> testTransEvent;
        private readonly DomainResult testResult = new DomainResult("test");

        public DomainEventHandlerSpec() : base(Substitute.For<IDomainEventStore<PublicSchemeId>>())
        {
            this.testTransEvent = new TransportMessage<SchemePublishedEvent, PublicSchemeId>(null, new Guid(), DateTime.UtcNow);
            this.domainEventAccessor.SaveEvent(this.testTransEvent).Returns(this.testResult);
        }

        [It(nameof(Handle))]
        public async Task Should_call_DomainEventAccessor__SaveEvent()
        {
            var result = await this.Handle(this.testTransEvent, new CancellationToken());

            await this.domainEventAccessor.Received(1).SaveEvent(this.testTransEvent);
        }

        [It(nameof(Handle))]
        public async Task Should_return_result_from_call_to_DomainEventAccessor__SaveEvent()
        {
            var result = await this.Handle(this.testTransEvent, new CancellationToken());

            result.Should().BeSameAs(this.testResult);
        }
    }
}
