using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Requests;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using MidnightLizard.Schemes.Processor.Configuration;
using MidnightLizard.Schemes.Testing;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;

using ITransRequest = MidnightLizard.Schemes.Domain.Common.Messaging.ITransportMessage<MidnightLizard.Schemes.Domain.Common.Messaging.BaseMessage>;
using TransRequest = MidnightLizard.Schemes.Domain.Common.Messaging.TransportMessage<MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Requests.SchemePublishRequest, MidnightLizard.Schemes.Domain.PublicSchemeAggregate.PublicSchemeId>;

namespace MidnightLizard.Schemes.Processor.Application.DomainRequestHandlers
{
    public class SchemePublishRequestHandlerSpec : SchemePublishRequestHandler
    {
        private readonly PublicScheme testScheme = Substitute.For<PublicScheme>();
        private readonly SchemePublishRequest testRequest = Substitute.For<SchemePublishRequest>();
        private static int handle_CallCount;

        protected SchemePublishRequestHandlerSpec() : base(
            Substitute.For<IOptions<AggregatesConfig>>(),
            Substitute.For<IMemoryCache>(),
            Substitute.For<IDomainEventDispatcher<PublicSchemeId>>(),
            Substitute.For<IAggregateSnapshotAccessor<PublicScheme, PublicSchemeId>>(),
            Substitute.For<IDomainEventStore<PublicSchemeId>>())
        {
            this.testScheme.Id.Returns(new PublicSchemeId());
            this.testRequest.PublisherId.Returns(new PublisherId());
            this.testRequest.AggregateId.Returns(this.testScheme.Id);
        }

        public override async Task<DomainResult> Handle(TransRequest transRequest, CancellationToken cancellationToken)
        {
            SchemePublishRequestHandlerSpec.handle_CallCount++;
            return DomainResult.Ok;
        }

        public class HandleDomainRequestSpec : SchemePublishRequestHandlerSpec
        {
            public HandleDomainRequestSpec() : base()
            {
            }

            [It(nameof(HandleDomainRequest))]
            public void Should_call_PublicScheme__Publish_with_corresponding_parameters()
            {
                this.HandleDomainRequest(this.testScheme, this.testRequest, new CancellationToken());

                this.testScheme.Received(1).Publish(this.testRequest.PublisherId, Arg.Any<ColorScheme>());
            }
        }

        public class MediatorSpec : SchemePublishRequestHandlerSpec
        {
            private readonly IMediator mediator;
            private readonly ITransRequest testTransRequest = new TransRequest(new SchemePublishRequest(), Guid.NewGuid(), DateTime.UtcNow);

            public MediatorSpec()
            {
                this.mediator = StartupStub.Resolve<IMediator>();
            }

            [It(nameof(Mediator))]
            public async Task Should_handle_Request()
            {
                SchemePublishRequestHandlerSpec.handle_CallCount = 0;

                var result = await this.mediator.Send(testTransRequest);

                SchemePublishRequestHandlerSpec.handle_CallCount.Should().Be(1);
            }
        }
    }
}
