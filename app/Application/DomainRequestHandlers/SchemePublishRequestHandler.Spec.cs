using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using MidnightLizard.Schemes.Processor.Configuration;
using MidnightLizard.Schemes.Tests;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Processor.Application.DomainRequestHandlers
{
    public class SchemePublishRequestHandlerSpec : SchemePublishRequestHandler
    {
        private readonly PublicScheme testScheme = Substitute.For<PublicScheme>();
        private readonly SchemePublishRequest testRequest = Substitute.For<SchemePublishRequest>();

        protected SchemePublishRequestHandlerSpec() : base(
            Substitute.For<IOptions<AggregatesConfig>>(),
            Substitute.For<IMemoryCache>(),
            Substitute.For<IDomainEventsDispatcher<PublicSchemeId>>(),
            Substitute.For<IAggregateSnapshotAccessor<PublicScheme, PublicSchemeId>>(),
            Substitute.For<IDomainEventsAccessor<PublicSchemeId>>())
        {
            this.testScheme.Id.Returns(new PublicSchemeId());
            this.testRequest.PublisherId.Returns(new PublisherId());
            this.testRequest.AggregateId.Returns(this.testScheme.Id);
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
    }
}
