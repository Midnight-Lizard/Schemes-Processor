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
        private int handleDomainRequest_CallCount = 0;
        private readonly Stub<IOptions<AggregatesConfig>> cacheConfigStub;
        private readonly Stub<IMemoryCache> memoryCacheStub;
        private readonly Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub;
        private readonly Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub;
        private readonly Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub;

        private readonly List<DomainEvent<PublicSchemeId>> testEvents =
            new List<DomainEvent<PublicSchemeId>>
            {
                new SchemePublishedEvent(),
                new SchemePublishedEvent()
            };
        private readonly AggregatesConfig testCacheConfig =
            new AggregatesConfig
            {
                AGGREGATES_CACHE_ENABLED = true,
                AGGREGATES_CACHE_SLIDING_EXPIRATION_SECONDS = 10,
                AGGREGATES_CACHE_ABSOLUTE_EXPIRATION_SECONDS = 60,
                AGGREGATES_MAX_EVENTS_COUNT = 1
            };
        private PublicScheme testScheme;
        private Mock<PublicScheme> testSchemeStub = new Mock<PublicScheme>();
        private readonly Mock<ICacheEntry> cacheEntryStub = new Mock<ICacheEntry>();

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
            testSchemeStub.SetupGet(s => s.Id).Returns(new PublicSchemeId());

            this.testScheme = testSchemeStub.Object;

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

        protected override void HandleDomainRequest(PublicScheme aggregate, DomainRequest<PublicSchemeId> request, CancellationToken cancellationToken
            )
        {
            Assert.Equal(aggregate, this.testScheme);
            Assert.Equal(aggregate.Id, request.AggregateId);

            handleDomainRequest_CallCount++;
        }

        public class DispatchDomainEventsSpec : AggregateRequestHandlerSpec
        {
            public DispatchDomainEventsSpec(Stub<IMapper> mapperStub, Stub<IOptions<AggregatesConfig>> cacheConfigStub, Stub<IMemoryCache> memoryCacheStub, Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub, Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub, Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub
                ) : base(mapperStub, cacheConfigStub, memoryCacheStub, dispatcherStub, snapshotStub, eventsAccessorStub)
            {
                this.testSchemeStub.Setup(s => s.ReleaseEvents()).Returns(this.testEvents).Verifiable();
                this.dispatcherStub
                     .Setup(d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()))
                     .ReturnsAsync(DomainResult.Ok);
            }

            [It(nameof(DispatchDomainEvents),
                nameof(Should_call_Aggregate__ReleaseEvents))]
            public async Task Should_call_Aggregate__ReleaseEvents(
                )
            {
                var results = await this.DispatchDomainEvents(this.testScheme);
                this.testSchemeStub.Verify();
            }

            [It(nameof(DispatchDomainEvents),
                nameof(Should_return_successful_results))]
            public async Task Should_return_successful_results(
                )
            {
                var results = await this.DispatchDomainEvents(this.testScheme);
                Assert.All(results, result => Assert.False(result.Value.HasError));
            }

            [It(nameof(DispatchDomainEvents),
                nameof(Should_return_results_for_each_Event_returned_from_Aggregate__ReleaseEvents))]
            public async Task Should_return_results_for_each_Event_returned_from_Aggregate__ReleaseEvents(
                )
            {
                var results = await this.DispatchDomainEvents(this.testScheme);
                Assert.Equal(this.testEvents.Count, results.Count);
            }

            [It(nameof(DispatchDomainEvents),
                nameof(Should_call_DispatchEvent_for_each_Event_returned_from_Aggregate__ReleaseEvents))]
            public async Task Should_call_DispatchEvent_for_each_Event_returned_from_Aggregate__ReleaseEvents(
                )
            {
                var results = await this.DispatchDomainEvents(this.testScheme);
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

                var results = await this.DispatchDomainEvents(this.testScheme);

                Assert.Single(results, r => r.Value.HasError);
                dispatcherStub.Verify(d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()), Times.Once);
            }

            [It(nameof(DispatchDomainEvents),
               nameof(Should_update_EventOffset_of_Aggregate))]
            public async Task Should_update_EventOffset_of_Aggregate(
               )
            {
                var results = await this.DispatchDomainEvents(this.testScheme);

                this.testSchemeStub.Verify(
                    s => s.ApplyEventOffset(It.IsAny<DomainEvent<PublicSchemeId>>()),
                    Times.Exactly(results.Count(r => !r.Value.HasError)));
            }
        }

        public class GetAggregateSnapshotSpec : AggregateRequestHandlerSpec
        {
            private AggregateResult<PublicScheme, PublicSchemeId> snapshotReadResult;

            public GetAggregateSnapshotSpec(Stub<IMapper> mapperStub, Stub<IOptions<AggregatesConfig>> cacheConfigStub, Stub<IMemoryCache> memoryCacheStub, Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub, Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub, Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub
                ) : base(mapperStub, cacheConfigStub, memoryCacheStub, dispatcherStub, snapshotStub, eventsAccessorStub)
            {
                this.snapshotReadResult = new AggregateResult<PublicScheme, PublicSchemeId>(testScheme);
                this.memoryCacheStub.Setup(s => s.TryGetValue(testScheme.Id, out It.Ref<object>.IsAny)).Returns(false);
                this.memoryCacheStub.Setup(s => s.CreateEntry(testScheme.Id)).Returns(cacheEntryStub.Object);
                this.snapshotStub.Setup(s => s.Read(testScheme.Id)).ReturnsAsync(() => this.snapshotReadResult);
                this.cacheConfigStub.SetupGet(s => s.Value).Returns(this.testCacheConfig);
            }

            [It(nameof(GetAggregateSnapshot),
                nameof(Should_set_up_correct_SlidingExpiration_for_a_new_MemoryCacheEntry))]
            public async Task Should_return_AggregateResult_from_Snapshot(
                )
            {
                var result = await this.GetAggregateSnapshot(testScheme.Id);
                Assert.Equal(this.snapshotReadResult, result);
            }

            [It(nameof(GetAggregateSnapshot),
               nameof(Should_call_AggregateSnapshot__Read_with_specified_AggregateId))]
            public async Task Should_call_AggregateSnapshot__Read_with_specified_AggregateId(
               )
            {
                var result = await this.GetAggregateSnapshot(testScheme.Id);
                this.snapshotStub.Verify(s => s.Read(testScheme.Id), Times.Once);
            }

            [It(nameof(GetAggregateSnapshot),
               nameof(Should_create_a_new_MemoryCacheEntry_for_specified_AggregateId))]
            public async Task Should_create_a_new_MemoryCacheEntry_for_specified_AggregateId(
               )
            {
                var result = await this.GetAggregateSnapshot(testScheme.Id);
                this.memoryCacheStub.Verify(s => s.CreateEntry(testScheme.Id), Times.Once);
            }

            [It(nameof(GetAggregateSnapshot),
                nameof(Should_set_up_correct_SlidingExpiration_for_a_new_MemoryCacheEntry))]
            public async Task Should_set_up_correct_SlidingExpiration_for_a_new_MemoryCacheEntry(
                )
            {
                var result = await this.GetAggregateSnapshot(testScheme.Id);
                this.cacheEntryStub.VerifySet(
                    s => s.SlidingExpiration = TimeSpan.FromSeconds(this.testCacheConfig.AGGREGATES_CACHE_SLIDING_EXPIRATION_SECONDS),
                    Times.Once);
            }

            [It(nameof(GetAggregateSnapshot),
                nameof(Should_set_up_some_AbsoluteExpiration_for_a_new_MemoryCacheEntry))]
            public async Task Should_set_up_some_AbsoluteExpiration_for_a_new_MemoryCacheEntry(
                )
            {
                var result = await this.GetAggregateSnapshot(testScheme.Id);
                this.cacheEntryStub.VerifySet(
                    s => s.AbsoluteExpiration = It.IsAny<DateTimeOffset>(),
                    Times.Once);
            }

            [It(nameof(GetAggregateSnapshot),
                nameof(Should_return_Error_if_AggregateSnapshot_returns_Error))]
            public async Task Should_return_Error_if_AggregateSnapshot_returns_Error(
                )
            {
                this.snapshotReadResult = new AggregateResult<PublicScheme, PublicSchemeId>("error");

                var result = await this.GetAggregateSnapshot(testScheme.Id);

                Assert.Equal(this.snapshotReadResult, result);
            }

            [It(nameof(GetAggregateSnapshot),
                nameof(Should_not_cache_if_AggregateSnapshot_returns_Error))]
            public async Task Should_not_cache_if_AggregateSnapshot_returns_Error(
                )
            {
                this.snapshotReadResult = new AggregateResult<PublicScheme, PublicSchemeId>("error");

                var result = await this.GetAggregateSnapshot(testScheme.Id);

                this.memoryCacheStub.Verify(s => s.CreateEntry(testScheme.Id), Times.Never);
            }

            [It(nameof(GetAggregateSnapshot),
                nameof(Should_not_read_from_MemoryCache_if_it_is_disabled))]
            public async Task Should_not_read_from_MemoryCache_if_it_is_disabled(
                )
            {
                this.testCacheConfig.AGGREGATES_CACHE_ENABLED = false;

                var result = await this.GetAggregateSnapshot(testScheme.Id);

                this.memoryCacheStub.Verify(s => s.TryGetValue(testScheme.Id, out It.Ref<object>.IsAny), Times.Never);
            }

            [It(nameof(GetAggregateSnapshot),
                nameof(Should_not_write_to_MemoryCache_if_it_is_disabled))]
            public async Task Should_not_write_to_MemoryCache_if_it_is_disabled(
                )
            {
                this.testCacheConfig.AGGREGATES_CACHE_ENABLED = false;

                var result = await this.GetAggregateSnapshot(testScheme.Id);

                this.memoryCacheStub.Verify(s => s.CreateEntry(testScheme.Id), Times.Never);
            }
        }

        public class GetAggregateSpec : AggregateRequestHandlerSpec
        {
            private AggregateResult<PublicScheme, PublicSchemeId> snapshotReadResult;
            private DomainEventsResult<PublicSchemeId> eventsReadResult;

            public GetAggregateSpec(Stub<IMapper> mapperStub, Stub<IOptions<AggregatesConfig>> cacheConfigStub, Stub<IMemoryCache> memoryCacheStub, Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub, Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub, Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub
                ) : base(mapperStub, cacheConfigStub, memoryCacheStub, dispatcherStub, snapshotStub, eventsAccessorStub)
            {
                this.snapshotReadResult = new AggregateResult<PublicScheme, PublicSchemeId>(this.testScheme);
                this.eventsReadResult = new DomainEventsResult<PublicSchemeId>(this.testEvents);

                this.cacheConfigStub
                    .SetupGet(s => s.Value)
                    .Returns(this.testCacheConfig);

                this.snapshotStub
                    .Setup(s => s.Read(testScheme.Id))
                    .ReturnsAsync(() => this.snapshotReadResult);

                this.eventsAccessorStub
                    .Setup(d => d.Read(testScheme))
                    .ReturnsAsync(() => eventsReadResult);

                this.memoryCacheStub
                    .Setup(s => s.CreateEntry(this.testScheme.Id))
                    .Returns(cacheEntryStub.Object);
            }

            [It(nameof(GetAggregate),
                nameof(Should_return_Error_when_Snapshot_returns_Error))]
            public async Task Should_return_Error_when_Snapshot_returns_Error()
            {
                this.snapshotReadResult = new AggregateResult<PublicScheme, PublicSchemeId>("error");

                var result = await this.GetAggregate(testScheme.Id);

                Assert.Equal(this.snapshotReadResult, result);
            }

            [It(nameof(GetAggregate),
                nameof(Should_return_Error_when_ReadDomainEvents_returns_Error))]
            public async Task Should_return_Error_when_ReadDomainEvents_returns_Error()
            {
                this.eventsReadResult = new DomainEventsResult<PublicSchemeId>("error");

                var result = await this.GetAggregate(testScheme.Id);

                Assert.Equal(this.eventsReadResult, result);
            }

            [It(nameof(GetAggregate),
                nameof(Should_call_AggregateSnapshot__Read))]
            public async Task Should_call_AggregateSnapshot__Read()
            {
                var result = await this.GetAggregate(testScheme.Id);
                this.snapshotStub.Verify(s => s.Read(testScheme.Id), Times.Once);
            }

            [It(nameof(GetAggregate),
                nameof(Should_call_Aggregate__ReplayDomainEvents))]
            public async Task Should_call_Aggregate__ReplayDomainEvents()
            {
                var result = await this.GetAggregate(testScheme.Id);
                this.testSchemeStub.Verify(s => s.ReplayDomainEvents(this.testEvents, this.mapper), Times.Once);
            }

            [It(nameof(GetAggregate),
                nameof(Should_save_AggregateSnapshot_if_it_has_enough_Events))]
            public async Task Should_save_AggregateSnapshot_if_it_has_enough_Events()
            {
                var result = await this.GetAggregate(this.testScheme.Id);
                this.snapshotStub.Verify(s => s.Save(this.testScheme), Times.Once);
            }
        }

        public class HandleSpec : AggregateRequestHandlerSpec
        {
            private DomainResult dispatchEventResult = DomainResult.Ok;
            private int dispatchDomainEvents_CallCount = 0;

            protected override Task<Dictionary<DomainEvent<PublicSchemeId>, DomainResult>> DispatchDomainEvents(IEventSourced<PublicSchemeId> aggregate)
            {
                Assert.Equal(this.testScheme, aggregate);
                dispatchDomainEvents_CallCount++;
                return base.DispatchDomainEvents(aggregate);
            }

            public HandleSpec(Stub<IMapper> mapperStub, Stub<IOptions<AggregatesConfig>> cacheConfigStub, Stub<IMemoryCache> memoryCacheStub, Stub<IDomainEventsDispatcher<PublicSchemeId>> dispatcherStub, Stub<IAggregateSnapshot<PublicScheme, PublicSchemeId>> snapshotStub, Stub<IDomainEventsAccessor<PublicSchemeId>> eventsAccessorStub
                ) : base(mapperStub, cacheConfigStub, memoryCacheStub, dispatcherStub, snapshotStub, eventsAccessorStub)
            {

                this.testSchemeStub
                    .Setup(s => s.ReleaseEvents())
                    .Returns(this.testEvents);

                this.dispatcherStub
                     .Setup(d => d.DispatchEvent(It.IsAny<SchemePublishedEvent>()))
                     .ReturnsAsync(() => dispatchEventResult);

                this.cacheConfigStub
                    .SetupGet(s => s.Value)
                    .Returns(this.testCacheConfig);

                this.memoryCacheStub
                    .Setup(s => s.CreateEntry(this.testScheme.Id))
                    .Returns(cacheEntryStub.Object);

                this.memoryCacheStub
                    .Setup(s => s.Remove(this.testScheme.Id));

                this.eventsAccessorStub
                    .Setup(d => d.Read(this.testScheme))
                    .ReturnsAsync(new DomainEventsResult<PublicSchemeId>(this.testEvents));

                this.snapshotStub
                    .Setup(s => s.Read(this.testScheme.Id))
                    .ReturnsAsync(new AggregateResult<PublicScheme, PublicSchemeId>(this.testScheme));
            }

            [It(nameof(Handle),
                nameof(Should_return_Error_if_GetAggregate_returns_Error))]
            public async Task Should_return_Error_if_GetAggregate_returns_Error()
            {
                var expectedResult = new AggregateResult<PublicScheme, PublicSchemeId>("error");
                this.snapshotStub.Setup(s => s.Read(this.testScheme.Id)).ReturnsAsync(expectedResult);

                var result = await this.Handle(new SchemePublishRequest(this.testScheme.Id), new CancellationToken());

                Assert.Equal(expectedResult, result);
            }

            [It(nameof(Handle),
                nameof(Should_call_HandleDomainRequest))]
            public async Task Should_call_HandleDomainRequest()
            {
                var result = await this.Handle(new SchemePublishRequest(this.testScheme.Id), new CancellationToken());

                Assert.Equal(1, this.handleDomainRequest_CallCount);
            }

            [It(nameof(Handle),
                nameof(Should_call_DispatchDomainEvents))]
            public async Task Should_call_DispatchDomainEvents()
            {
                var result = await this.Handle(new SchemePublishRequest(this.testScheme.Id), new CancellationToken());

                Assert.Equal(1, this.dispatchDomainEvents_CallCount);
            }

            [It(nameof(Handle),
                nameof(Should_return_Error_from_DispatchDomainEvents))]
            public async Task Should_return_Error_from_DispatchDomainEvents()
            {
                this.dispatchEventResult = DomainResult.UnknownError;

                var result = await this.Handle(new SchemePublishRequest(this.testScheme.Id), new CancellationToken());

                Assert.Equal(DomainResult.UnknownError, result);
            }

            [It(nameof(Handle),
                nameof(Should_remove_Aggregate_from_MemoryCache_if_failed_to_DispatchEvent))]
            public async Task Should_remove_Aggregate_from_MemoryCache_if_failed_to_DispatchEvent()
            {
                this.dispatchEventResult = DomainResult.UnknownError;

                var result = await this.Handle(new SchemePublishRequest(this.testScheme.Id), new CancellationToken());

                this.memoryCacheStub.Verify(s => s.Remove(this.testScheme.Id), Times.Once);
            }
        }
    }
}
