using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Processor.Configuration;
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
        IClassFixture<Stub<IDomainEventsAccessor<PublicSchemeId>>>,
        IClassFixture<Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>>>,
        IClassFixture<Stub<IMemoryCache>>,
        IClassFixture<Stub<IOptions<AggregatesCacheConfig>>>
    {
        protected readonly Stub<IOptions<AggregatesCacheConfig>> cacheConfigStub;
        protected readonly Stub<IMemoryCache> memoryCacheStub;
        protected readonly Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub;
        protected readonly Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub;
        protected readonly Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub;

        protected readonly List<DomainEvent<PublicSchemeId>> testEvents =
            new List<DomainEvent<PublicSchemeId>>
            {
                new DomainEvent<PublicSchemeId>(),
                new DomainEvent<PublicSchemeId>()
            };
        protected readonly AggregatesCacheConfig testCacheConfig =
            new AggregatesCacheConfig
            {
                AGGREGATES_CACHE_ENABLED = true,
                AGGREGATES_CACHE_SLIDING_EXPIRATION_SECONDS = 10,
                AGGREGATES_CACHE_ABSOLUTE_EXPIRATION_SECONDS = 60
            };

        public AggregateRequestHandlerSpec(
                Stub<IOptions<AggregatesCacheConfig>> cacheConfigStub,
                Stub<IMemoryCache> memoryCacheStub,
                Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub,
                Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub,
                Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub) :
                base(cacheConfigStub.Object, memoryCacheStub.Object, dispatcherStub.Object,
                    snapshotStub.Object, eventsAccessorStub.Object)
        {
            this.cacheConfigStub = cacheConfigStub;
            this.memoryCacheStub = memoryCacheStub;
            this.dispatcherStub = dispatcherStub;
            this.snapshotStub = snapshotStub;
            this.eventsAccessorStub = eventsAccessorStub;

            this.eventsAccessorStub.Reset();
            this.snapshotStub.Reset();
            this.dispatcherStub.Reset();
            this.memoryCacheStub.Reset();
            this.cacheConfigStub.Reset();
        }

        public override Task<DomainResult> Handle(DomainRequest<PublicSchemeId> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [It(nameof(DispatchDomainEvents),
            nameof(Should_call_DomainEventsDispatcher_for_each_Event_in_Aggregate_and_return_a_List_of_results))]
        public async Task Should_call_DomainEventsDispatcher_for_each_Event_in_Aggregate_and_return_a_List_of_results(
            )
        {
            var scheme = new PublicScheme();
            scheme.Events.AddRange(new[] {
                new SchemePublishedEvent(),
                new SchemePublishedEvent()
            });
            this.dispatcherStub
                 .Setup(d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()))
                 .ReturnsAsync(DomainResult.Ok);
            var results = await this.DispatchDomainEvents(scheme);
            Assert.All(results, result => Assert.False(result.Value.HasError));
            dispatcherStub.Verify(
                d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()),
                Times.Exactly(scheme.Events.Count));
            Assert.Equal(scheme.Events.Count, results.Count);
        }

        [It(nameof(DispatchDomainEvents),
           nameof(Should_stop_dispatching_events_if_error_returned_by_EventsDispatcher))]
        public async Task Should_stop_dispatching_events_if_error_returned_by_EventsDispatcher(
           )
        {
            var scheme = new PublicScheme();
            scheme.Events.AddRange(new[] {
                new SchemePublishedEvent(),
                new SchemePublishedEvent()
            });
            this.dispatcherStub
                 .Setup(d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()))
                 .ReturnsAsync(DomainResult.Error);

            var results = await this.DispatchDomainEvents(scheme);

            Assert.Single(results);
            Assert.Single(results, r => r.Value.HasError);
            dispatcherStub.Verify(d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()), Times.Once);
        }

        [It(nameof(ReadDomainEvents),
            nameof(Should_call_DomainEventsAccessor_and_return_its_results))]
        public async Task Should_call_DomainEventsAccessor_and_return_its_results(
            )
        {
            var testScheme = new PublicScheme();
            this.eventsAccessorStub
                .Setup(d => d.Read(testScheme))
                .ReturnsAsync(new DomainEventsResult<PublicSchemeId>(testEvents));
            var results = await this.ReadDomainEvents(testScheme);
            eventsAccessorStub.Verify(d => d.Read(testScheme), Times.Once);
            Assert.Equal(this.testEvents.Count, results.Events.Count);
        }

        [It(nameof(GetAggregateSnapshot),
            nameof(Should_restore_SchemesAggregate_from_Snapshot_and_cache_it_in_memory))]
        public async Task Should_restore_SchemesAggregate_from_Snapshot_and_cache_it_in_memory(
            )
        {
            var testSchemeId = new PublicSchemeId();
            var cacheEntryStub = new Mock<ICacheEntry>();
            var expectedResult = new AggregateResult<PublicScheme, PublicSchemeId>(new PublicScheme());

            this.memoryCacheStub.Setup(s => s.TryGetValue(testSchemeId, out It.Ref<object>.IsAny)).Returns(false);
            this.memoryCacheStub.Setup(s => s.CreateEntry(testSchemeId)).Returns(cacheEntryStub.Object).Verifiable();
            this.snapshotStub.Setup(s => s.Read(testSchemeId)).ReturnsAsync(expectedResult).Verifiable();
            this.cacheConfigStub.SetupGet(s => s.Value).Returns(this.testCacheConfig);
            cacheEntryStub.SetupSet(s => s.SlidingExpiration = TimeSpan.FromSeconds(this.testCacheConfig.AGGREGATES_CACHE_SLIDING_EXPIRATION_SECONDS)).Verifiable();

            var result = await this.GetAggregateSnapshot(testSchemeId);

            Assert.Equal(expectedResult, result);
            this.snapshotStub.Verify();
            this.memoryCacheStub.Verify();
            cacheEntryStub.Verify();
        }

        [It(nameof(GetAggregateSnapshot),
            nameof(Should_not_cache_if_AggregateResult_HasError))]
        public async Task Should_not_cache_if_AggregateResult_HasError(
            )
        {
            var testSchemeId = new PublicSchemeId();
            var expectedResult = new AggregateResult<PublicScheme, PublicSchemeId>("error");

            this.memoryCacheStub.Setup(s => s.TryGetValue(testSchemeId, out It.Ref<object>.IsAny)).Returns(false);
            this.snapshotStub.Setup(s => s.Read(testSchemeId)).ReturnsAsync(expectedResult).Verifiable();
            this.cacheConfigStub.SetupGet(s => s.Value).Returns(this.testCacheConfig);

            var result = await this.GetAggregateSnapshot(testSchemeId);

            Assert.Equal(expectedResult, result);
            this.snapshotStub.Verify();
            this.memoryCacheStub.Verify(s => s.CreateEntry(testSchemeId), Times.Never);
        }

        [It(nameof(GetAggregateSnapshot),
            nameof(Should_not_cache_if_caching_disabled))]
        public async Task Should_not_cache_if_caching_disabled(
            )
        {
            var testSchemeId = new PublicSchemeId();
            var expectedResult = new AggregateResult<PublicScheme, PublicSchemeId>(new PublicScheme());
            this.testCacheConfig.AGGREGATES_CACHE_ENABLED = false;

            this.memoryCacheStub.Setup(s => s.TryGetValue(testSchemeId, out It.Ref<object>.IsAny)).Returns(false);
            this.snapshotStub.Setup(s => s.Read(testSchemeId)).ReturnsAsync(expectedResult).Verifiable();
            this.cacheConfigStub.SetupGet(s => s.Value).Returns(this.testCacheConfig);

            var result = await this.GetAggregateSnapshot(testSchemeId);

            Assert.Equal(expectedResult, result);
            this.snapshotStub.Verify();
            this.memoryCacheStub.Verify(s => s.TryGetValue(testSchemeId, out It.Ref<object>.IsAny), Times.Never);
            this.memoryCacheStub.Verify(s => s.CreateEntry(testSchemeId), Times.Never);
        }
    }
}
