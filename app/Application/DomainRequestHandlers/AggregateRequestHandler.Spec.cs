using AutoMapper;
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
        IClassFixture<Stub<IMapper>>,
        IClassFixture<Stub<IOptions<AggregatesConfig>>>
    {
        private int handleDomainRequestCallCount = 0;
        private readonly Stub<IOptions<AggregatesConfig>> cacheConfigStub;
        private readonly Stub<IMemoryCache> memoryCacheStub;
        private readonly Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub;
        private readonly Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub;
        private readonly Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub;

        protected readonly List<DomainEvent<PublicSchemeId>> testEvents =
            new List<DomainEvent<PublicSchemeId>>
            {
                new SchemePublishedEvent(),
                new SchemePublishedEvent()
            };
        protected readonly AggregatesConfig testCacheConfig =
            new AggregatesConfig
            {
                AGGREGATES_CACHE_ENABLED = true,
                AGGREGATES_CACHE_SLIDING_EXPIRATION_SECONDS = 10,
                AGGREGATES_CACHE_ABSOLUTE_EXPIRATION_SECONDS = 60,
                AGGREGATES_MAX_EVENTS_COUNT = 1
            };

        public AggregateRequestHandlerSpec(
            Stub<IMapper> mapperStub,
            Stub<IOptions<AggregatesConfig>> cacheConfigStub,
            Stub<IMemoryCache> memoryCacheStub,
            Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub,
            Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub,
            Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub) :
            base(mapperStub.Object, cacheConfigStub.Object, memoryCacheStub.Object, dispatcherStub.Object,
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

        protected override void HandleDomainRequest(PublicScheme aggregate, DomainRequest<PublicSchemeId> request, CancellationToken cancellationToken)
        {
            handleDomainRequestCallCount++;
        }

        public class DispatchDomainEventsSpec : AggregateRequestHandlerSpec
        {
            private readonly Stub<PublicScheme> schemeStub = new Stub<PublicScheme>();

            public DispatchDomainEventsSpec(Stub<IMapper> mapperStub, Stub<IOptions<AggregatesConfig>> cacheConfigStub, Stub<IMemoryCache> memoryCacheStub, Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub, Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub, Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub
                ) : base(mapperStub, cacheConfigStub, memoryCacheStub, dispatcherStub, snapshotStub, eventsAccessorStub)
            {
                schemeStub.Setup(s => s.ReleaseEvents()).Returns(this.testEvents).Verifiable();
                this.dispatcherStub
                     .Setup(d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()))
                     .ReturnsAsync(DomainResult.Ok);
            }

            [It(nameof(DispatchDomainEvents),
                nameof(Should_call_Aggregate__ReleaseEvents))]
            public async Task Should_call_Aggregate__ReleaseEvents(
                )
            {
                var results = await this.DispatchDomainEvents(schemeStub.Object);
                schemeStub.Verify();
            }

            [It(nameof(DispatchDomainEvents),
                nameof(Should_return_successful_results))]
            public async Task Should_return_successful_results(
                )
            {
                var results = await this.DispatchDomainEvents(schemeStub.Object);
                Assert.All(results, result => Assert.False(result.Value.HasError));
            }

            [It(nameof(DispatchDomainEvents),
                nameof(Should_return_results_for_each_Event_returned_from_Aggregate__ReleaseEvents))]
            public async Task Should_return_results_for_each_Event_returned_from_Aggregate__ReleaseEvents(
                )
            {
                var results = await this.DispatchDomainEvents(schemeStub.Object);
                Assert.Equal(this.testEvents.Count, results.Count);
            }

            [It(nameof(DispatchDomainEvents),
                nameof(Should_call_DispatchEvent_for_each_Event_returned_from_Aggregate__ReleaseEvents))]
            public async Task Should_call_DispatchEvent_for_each_Event_returned_from_Aggregate__ReleaseEvents(
                )
            {
                var results = await this.DispatchDomainEvents(schemeStub.Object);
                dispatcherStub.Verify(
                    d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()),
                    Times.Exactly(this.testEvents.Count));
            }

            [It(nameof(DispatchDomainEvents),
               nameof(Should_stop_dispatching_events_if_error_returned_by_EventsDispatcher))]
            public async Task Should_stop_dispatching_events_if_error_returned_by_EventsDispatcher(
               )
            {
                this.dispatcherStub
                     .Setup(d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()))
                     .ReturnsAsync(DomainResult.UnknownError);

                var results = await this.DispatchDomainEvents(schemeStub.Object);

                Assert.Single(results, r => r.Value.HasError);
                dispatcherStub.Verify(d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()), Times.Once);
            }

            [It(nameof(DispatchDomainEvents),
               nameof(Should_update_EventOffset_of_Aggregate))]
            public async Task Should_update_EventOffset_of_Aggregate(
               )
            {
                var results = await this.DispatchDomainEvents(schemeStub.Object);

                schemeStub.Verify(
                    s => s.ApplyEventOffset(It.IsAny<DomainEvent<PublicSchemeId>>()),
                    Times.Exactly(results.Count(r => !r.Value.HasError)));
            }
        }

        public class GetAggregateSnapshotSpec : AggregateRequestHandlerSpec
        {
            public GetAggregateSnapshotSpec(Stub<IMapper> mapperStub, Stub<IOptions<AggregatesConfig>> cacheConfigStub, Stub<IMemoryCache> memoryCacheStub, Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub, Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub, Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub
                ) : base(mapperStub, cacheConfigStub, memoryCacheStub, dispatcherStub, snapshotStub, eventsAccessorStub)
            {

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

        public class GetAggregateSpec : AggregateRequestHandlerSpec
        {
            public GetAggregateSpec(Stub<IMapper> mapperStub, Stub<IOptions<AggregatesConfig>> cacheConfigStub, Stub<IMemoryCache> memoryCacheStub, Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub, Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub, Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub
                ) : base(mapperStub, cacheConfigStub, memoryCacheStub, dispatcherStub, snapshotStub, eventsAccessorStub)
            {

            }

            [It(nameof(GetAggregate),
                nameof(Should_return_Error_when_Snapshot_returns_Error))]
            public async Task Should_return_Error_when_Snapshot_returns_Error()
            {
                var testSchemeId = new PublicSchemeId();
                var expectedResult = new AggregateResult<PublicScheme, PublicSchemeId>("error");
                this.cacheConfigStub.SetupGet(s => s.Value).Returns(this.testCacheConfig);
                this.snapshotStub.Setup(s => s.Read(testSchemeId)).ReturnsAsync(expectedResult).Verifiable();

                var result = await this.GetAggregate(testSchemeId);

                Assert.Equal(expectedResult, result);

                this.snapshotStub.Verify();
            }

            [It(nameof(GetAggregate),
                nameof(Should_return_Error_when_ReadDomainEvents_returns_Error))]
            public async Task Should_return_Error_when_ReadDomainEvents_returns_Error()
            {
                var testScheme = new PublicScheme();
                var expectedResult = new DomainEventsResult<PublicSchemeId>("error");
                var cacheEntryStub = new Mock<ICacheEntry>();

                this.cacheConfigStub
                    .SetupGet(s => s.Value)
                    .Returns(this.testCacheConfig);

                this.snapshotStub.Setup(s => s.Read(testScheme.Id))
                    .ReturnsAsync(new AggregateResult<PublicScheme, PublicSchemeId>(testScheme))
                    .Verifiable();

                this.eventsAccessorStub
                    .Setup(d => d.Read(testScheme))
                    .ReturnsAsync(expectedResult);

                this.memoryCacheStub
                    .Setup(s => s.CreateEntry(testScheme.Id))
                    .Returns(cacheEntryStub.Object).Verifiable();

                cacheEntryStub
                    .SetupSet(s => s.SlidingExpiration = TimeSpan.FromSeconds(this.testCacheConfig.AGGREGATES_CACHE_SLIDING_EXPIRATION_SECONDS))
                    .Verifiable();

                var result = await this.GetAggregate(testScheme.Id);

                Assert.Equal(expectedResult, result);
                this.snapshotStub.Verify();
                this.memoryCacheStub.Verify();
                cacheEntryStub.Verify();
            }

            [It(nameof(GetAggregate),
                nameof(Should_save_AggregateSnapshot_if_it_has_enough_Events))]
            public async Task Should_save_AggregateSnapshot_if_it_has_enough_Events()
            {
                var testSchemeStub = new Mock<PublicScheme>();
                var expectedResult = new AggregateResult<PublicScheme, PublicSchemeId>(testSchemeStub.Object);
                var cacheEntryStub = new Mock<ICacheEntry>();

                this.cacheConfigStub
                    .SetupGet(s => s.Value)
                    .Returns(this.testCacheConfig);

                this.snapshotStub.Setup(s => s.Read(testSchemeStub.Object.Id))
                    .ReturnsAsync(expectedResult)
                    .Verifiable();

                this.snapshotStub.Setup(s => s.Save(testSchemeStub.Object))
                    .ReturnsAsync(DomainResult.Ok)
                    .Verifiable();

                this.eventsAccessorStub
                    .Setup(d => d.Read(testSchemeStub.Object))
                    .ReturnsAsync(new DomainEventsResult<PublicSchemeId>(this.testEvents));

                this.memoryCacheStub
                    .Setup(s => s.CreateEntry(testSchemeStub.Object.Id))
                    .Returns(cacheEntryStub.Object).Verifiable();

                cacheEntryStub
                    .SetupSet(s => s.SlidingExpiration = TimeSpan.FromSeconds(this.testCacheConfig.AGGREGATES_CACHE_SLIDING_EXPIRATION_SECONDS))
                    .Verifiable();

                var result = await this.GetAggregate(testSchemeStub.Object.Id);

                Assert.Equal(expectedResult, result);
                this.snapshotStub.Verify();
                this.memoryCacheStub.Verify();
                cacheEntryStub.Verify();
                testSchemeStub.Verify(s => s.ReplayDomainEvents(this.testEvents, this.mapper), Times.Once);
            }
        }

        public class HandleSpec : AggregateRequestHandlerSpec
        {
            public HandleSpec(Stub<IMapper> mapperStub, Stub<IOptions<AggregatesConfig>> cacheConfigStub, Stub<IMemoryCache> memoryCacheStub, Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub, Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub, Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub
                ) : base(mapperStub, cacheConfigStub, memoryCacheStub, dispatcherStub, snapshotStub, eventsAccessorStub)
            {

            }

            [It(nameof(Handle),
                nameof(Should_return_Error_if_GetAggregate_returns_Error))]
            public async Task Should_return_Error_if_GetAggregate_returns_Error()
            {
                var testSchemeId = new PublicSchemeId();
                var expectedResult = new AggregateResult<PublicScheme, PublicSchemeId>("error");
                this.cacheConfigStub.SetupGet(s => s.Value).Returns(this.testCacheConfig);
                this.snapshotStub.Setup(s => s.Read(testSchemeId)).ReturnsAsync(expectedResult).Verifiable();

                var result = await this.Handle(new SchemePublishRequest(testSchemeId), new CancellationToken());

                Assert.Equal(expectedResult, result);

                this.snapshotStub.Verify();
            }

            [It(nameof(Handle),
                nameof(Should_return_Error_from_DispatchDomainEvents_and_remove_Aggregate_from_MemoryCache))]
            public async Task Should_return_Error_from_DispatchDomainEvents_and_remove_Aggregate_from_MemoryCache()
            {
                var cacheEntryStub = new Mock<ICacheEntry>();
                var testSchemeStub = new Stub<PublicScheme>();

                testSchemeStub
                    .Setup(s => s.ReleaseEvents())
                    .Returns(this.testEvents)
                    .Verifiable();

                this.dispatcherStub
                     .Setup(d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()))
                     .ReturnsAsync(DomainResult.UnknownError);

                this.cacheConfigStub
                    .SetupGet(s => s.Value)
                    .Returns(this.testCacheConfig);

                this.memoryCacheStub
                    .Setup(s => s.CreateEntry(testSchemeStub.Object.Id))
                    .Returns(cacheEntryStub.Object)
                    .Verifiable();

                this.memoryCacheStub
                    .Setup(s => s.Remove(testSchemeStub.Object.Id))
                    .Verifiable();

                this.eventsAccessorStub
                    .Setup(d => d.Read(testSchemeStub.Object))
                    .ReturnsAsync(new DomainEventsResult<PublicSchemeId>(this.testEvents));

                this.snapshotStub.Setup(s => s.Read(testSchemeStub.Object.Id))
                    .ReturnsAsync(new AggregateResult<PublicScheme, PublicSchemeId>(testSchemeStub.Object))
                    .Verifiable();

                var result = await this.Handle(new SchemePublishRequest(testSchemeStub.Object.Id), new CancellationToken());

                Assert.Equal(DomainResult.UnknownError, result);

                this.snapshotStub.Verify();
                testSchemeStub.Verify();
                this.memoryCacheStub.Verify();
            }
        }
    }
}
