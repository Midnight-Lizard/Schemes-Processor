using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Commons.Domain.Interfaces;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Requests;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using MidnightLizard.Schemes.Processor.Configuration;
using MidnightLizard.Testing.Utilities;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;

using ITransRequest = MidnightLizard.Commons.Domain.Messaging.ITransportMessage<MidnightLizard.Commons.Domain.Messaging.BaseMessage>;
using TransRequest = MidnightLizard.Commons.Domain.Messaging.TransportMessage<MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Requests.PublishSchemeRequest, MidnightLizard.Schemes.Domain.PublicSchemeAggregate.PublicSchemeId>;

namespace MidnightLizard.Schemes.Processor.Application.DomainRequestHandlers
{
    public class SchemePublishRequestHandlerSpec : SchemePublishRequestHandler
    {
        private readonly PublicScheme testScheme = Substitute.For<PublicScheme>();
        private readonly PublishSchemeRequest testRequest = Substitute.For<PublishSchemeRequest>();
        private readonly UserId testUserId = new UserId("test-user-id");
        private static int handle_CallCount;

        protected SchemePublishRequestHandlerSpec() : base(
            Substitute.For<IOptions<AggregatesConfig>>(),
            Substitute.For<IMemoryCache>(),
            Substitute.For<IDomainEventDispatcher<PublicSchemeId>>(),
            Substitute.For<IAggregateSnapshotAccessor<PublicScheme, PublicSchemeId>>(),
            Substitute.For<IDomainEventStore<PublicSchemeId>>())
        {
            this.testScheme.Id.Returns(new PublicSchemeId());
            this.testRequest.AggregateId.Returns(this.testScheme.Id);
        }

        public override Task<DomainResult> Handle(TransRequest transRequest, CancellationToken cancellationToken)
        {
            SchemePublishRequestHandlerSpec.handle_CallCount++;
            return Task.FromResult(DomainResult.Ok);
        }

        public class HandleDomainRequestSpec : SchemePublishRequestHandlerSpec
        {
            public HandleDomainRequestSpec() : base()
            {
            }

            [It(nameof(HandleDomainRequest))]
            public void Should_call_PublicScheme__Publish_with_corresponding_parameters()
            {
                this.HandleDomainRequest(this.testScheme, this.testRequest, this.testUserId, new CancellationToken());

                this.testScheme.Received(1).Publish(new PublisherId(this.testUserId.Value), Arg.Any<ColorScheme>());
            }
        }

        public class MediatorSpec : SchemePublishRequestHandlerSpec
        {
            private readonly IMediator mediator;
            private readonly ITransRequest testTransRequest;

            public MediatorSpec()
            {
                this.testTransRequest = new TransRequest(new PublishSchemeRequest(), Guid.NewGuid(), DateTime.UtcNow, this.testUserId);
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
