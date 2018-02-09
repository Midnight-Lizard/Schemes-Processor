using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.Common.Results;
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
    public class AggregateRequestHandlerSpec : AggregateRequestHandler<PublicScheme, DomainRequest<PublicSchemeId>, PublicSchemeId>
    {
        private int handleDomainRequest_CallCount = 0;

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
        private readonly PublicScheme testScheme = Substitute.For<PublicScheme>();
        private readonly ICacheEntry cacheEntry = Substitute.For<ICacheEntry>();

        public AggregateRequestHandlerSpec() : base(
            Substitute.For<IMapper>(),
            Substitute.For<IOptions<AggregatesConfig>>(),
            Substitute.For<IMemoryCache>(),
            Substitute.For<IDomainEventsDispatcher<PublicSchemeId>>(),
            Substitute.For<IAggregateSnapshot<PublicScheme, PublicSchemeId>>(),
            Substitute.For<IDomainEventsAccessor<PublicSchemeId>>())
        {
            this.testScheme.Id.Returns(new PublicSchemeId());
            this.testEvents.ForEach(e => e.SetUp(this.testScheme.Id));

            this.eventsAccessor.ClearReceivedCalls();
            this.aggregateSnapshot.ClearReceivedCalls();
            this.eventsDispatcher.ClearReceivedCalls();
            this.memoryCache.ClearReceivedCalls();
            this.aggregatesConfig.ClearReceivedCalls();
        }

        protected override void HandleDomainRequest(PublicScheme aggregate, DomainRequest<PublicSchemeId> request, CancellationToken cancellationToken
            )
        {
            aggregate.Should().BeSameAs(this.testScheme);
            request.AggregateId.Should().BeSameAs(aggregate.Id);

            handleDomainRequest_CallCount++;
        }

        public class DispatchDomainEventsSpec : AggregateRequestHandlerSpec
        {
            public DispatchDomainEventsSpec() : base()
            {
                this.testScheme.ReleaseEvents()
                    .Returns(this.testEvents);

                this.eventsDispatcher.DispatchEvent(Arg.Any<SchemePublishedEvent>())
                    .Returns(DomainResult.Ok);
            }

            [It(nameof(DispatchDomainEvents))]
            public async Task Should_call_Aggregate__ReleaseEvents(
                )
            {
                var results = await this.DispatchDomainEvents(this.testScheme);

                this.testScheme.Received(1).ReleaseEvents();
            }

            [It(nameof(DispatchDomainEvents))]
            public async Task Should_return_successful_results(
                )
            {
                var results = await this.DispatchDomainEvents(this.testScheme);

                results.Values.Should().NotContain(val => val.HasError);
            }

            [It(nameof(DispatchDomainEvents))]
            public async Task Should_return_results_for_each_Event_returned_from_Aggregate__ReleaseEvents(
                )
            {
                var results = await this.DispatchDomainEvents(this.testScheme);

                results.Should().HaveCount(this.testEvents.Count);
            }

            [It(nameof(DispatchDomainEvents))]
            public async Task Should_call_DispatchEvent_for_each_Event_returned_from_Aggregate__ReleaseEvents(
                )
            {
                var results = await this.DispatchDomainEvents(this.testScheme);

                await this.eventsDispatcher
                    .Received(this.testEvents.Count)
                    .DispatchEvent(Arg.Any<SchemePublishedEvent>());
            }

            [It(nameof(DispatchDomainEvents))]
            public async Task Should_stop_dispatching_events_if_error_returned_by_EventsDispatcher(
               )
            {
                this.eventsDispatcher
                    .DispatchEvent(Arg.Any<SchemePublishedEvent>())
                    .Returns(DomainResult.UnknownError);

                var results = await this.DispatchDomainEvents(this.testScheme);

                results.Values.Should().ContainSingle(r => r.HasError);
                await this.eventsDispatcher.Received(1)
                    .DispatchEvent(Arg.Any<SchemePublishedEvent>());
            }

            [It(nameof(DispatchDomainEvents))]
            public async Task Should_update_EventOffset_of_Aggregate(
               )
            {
                var results = await this.DispatchDomainEvents(this.testScheme);

                this.testScheme.Received(results.Count(r => !r.Value.HasError))
                    .ApplyEventOffset(Arg.Any<SchemePublishedEvent>());
            }
        }

        public class GetAggregateSnapshotSpec : AggregateRequestHandlerSpec
        {
            private AggregateResult<PublicScheme, PublicSchemeId> snapshotReadResult;

            public GetAggregateSnapshotSpec() : base()
            {
                this.snapshotReadResult = new AggregateResult<PublicScheme, PublicSchemeId>(testScheme);

                this.memoryCache.TryGetValue(testScheme.Id, out var some)
                    .Returns(false);

                this.memoryCache.CreateEntry(testScheme.Id)
                    .Returns(cacheEntry);

                this.aggregateSnapshot.Read(testScheme.Id)
                    .Returns(x => this.snapshotReadResult);

                this.aggregatesConfig.Value
                    .Returns(this.testCacheConfig);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_return_AggregateResult_from_Snapshot(
                )
            {
                var result = await this.GetAggregateSnapshot(testScheme.Id);

                result.Should().BeSameAs(this.snapshotReadResult);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_call_AggregateSnapshot__Read_with_specified_AggregateId(
                )
            {
                var result = await this.GetAggregateSnapshot(testScheme.Id);

                await this.aggregateSnapshot.Received(1).Read(testScheme.Id);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_create_a_new_MemoryCacheEntry_for_specified_AggregateId(
                )
            {
                var result = await this.GetAggregateSnapshot(testScheme.Id);

                this.memoryCache.Received(1).CreateEntry(testScheme.Id);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_set_up_correct_SlidingExpiration_for_a_new_MemoryCacheEntry(
                )
            {
                var result = await this.GetAggregateSnapshot(testScheme.Id);

                this.cacheEntry.Received(1).SlidingExpiration =
                    TimeSpan.FromSeconds(this.testCacheConfig.AGGREGATES_CACHE_SLIDING_EXPIRATION_SECONDS);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_set_up_correct_AbsoluteExpiration_for_a_new_MemoryCacheEntry(
                )
            {
                var result = await this.GetAggregateSnapshot(testScheme.Id);

                var now = DateTimeOffset.Now;

                this.cacheEntry.Received(1).AbsoluteExpiration =
                    Arg.Is<DateTimeOffset>(dt => dt > now &&
                        dt < now.AddSeconds(this.testCacheConfig.AGGREGATES_CACHE_ABSOLUTE_EXPIRATION_SECONDS));
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_return_Error_if_AggregateSnapshot_returns_Error(
                )
            {
                this.snapshotReadResult = new AggregateResult<PublicScheme, PublicSchemeId>("error");

                var result = await this.GetAggregateSnapshot(testScheme.Id);

                result.Should().BeSameAs(this.snapshotReadResult);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_not_cache_if_AggregateSnapshot_returns_Error(
                )
            {
                this.snapshotReadResult = new AggregateResult<PublicScheme, PublicSchemeId>("error");

                var result = await this.GetAggregateSnapshot(testScheme.Id);

                this.memoryCache.DidNotReceive().CreateEntry(Arg.Any<PublicSchemeId>());
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_not_read_from_MemoryCache_if_it_is_disabled(
                )
            {
                this.testCacheConfig.AGGREGATES_CACHE_ENABLED = false;

                var result = await this.GetAggregateSnapshot(testScheme.Id);

                this.memoryCache.DidNotReceiveWithAnyArgs().TryGetValue(testScheme.Id, out var some);
            }

            [It(nameof(GetAggregateSnapshot))]
            public async Task Should_not_write_to_MemoryCache_if_it_is_disabled(
                )
            {
                this.testCacheConfig.AGGREGATES_CACHE_ENABLED = false;

                var result = await this.GetAggregateSnapshot(testScheme.Id);

                this.memoryCache.DidNotReceiveWithAnyArgs().CreateEntry(null);
            }
        }

        public class GetAggregateSpec : AggregateRequestHandlerSpec
        {
            private AggregateResult<PublicScheme, PublicSchemeId> snapshotReadResult;
            private DomainEventsResult<PublicSchemeId> eventsReadResult;

            public GetAggregateSpec() : base()
            {
                this.snapshotReadResult = new AggregateResult<PublicScheme, PublicSchemeId>(this.testScheme);
                this.eventsReadResult = new DomainEventsResult<PublicSchemeId>(this.testEvents);

                this.aggregatesConfig.Value
                    .Returns(this.testCacheConfig);

                this.aggregateSnapshot.Read(testScheme.Id)
                    .Returns(x => this.snapshotReadResult);

                this.eventsAccessor.Read(testScheme)
                    .Returns(x => eventsReadResult);

                this.memoryCache.CreateEntry(this.testScheme.Id)
                    .Returns(this.cacheEntry);
            }

            [It(nameof(GetAggregate))]
            public async Task Should_return_Error_when_Snapshot_returns_Error(
                )
            {
                this.snapshotReadResult = new AggregateResult<PublicScheme, PublicSchemeId>("error");

                var result = await this.GetAggregate(testScheme.Id);

                result.Should().BeSameAs(this.snapshotReadResult);
            }

            [It(nameof(GetAggregate))]
            public async Task Should_return_Error_when_ReadDomainEvents_returns_Error(
                )
            {
                this.eventsReadResult = new DomainEventsResult<PublicSchemeId>("error");

                var result = await this.GetAggregate(testScheme.Id);

                result.Should().BeSameAs(this.eventsReadResult);
            }

            [It(nameof(GetAggregate))]
            public async Task Should_call_AggregateSnapshot__Read(
                )
            {
                var result = await this.GetAggregate(testScheme.Id);

                await this.aggregateSnapshot.Received(1).Read(testScheme.Id);
            }

            [It(nameof(GetAggregate))]
            public async Task Should_call_Aggregate__ReplayDomainEvents(
                )
            {
                var result = await this.GetAggregate(testScheme.Id);

                this.testScheme.Received(1).ReplayDomainEvents(this.testEvents, this.mapper);
            }

            [It(nameof(GetAggregate))]
            public async Task Should_save_AggregateSnapshot_if_it_has_too_many_Events(
                )
            {
                var result = await this.GetAggregate(this.testScheme.Id);

                await this.aggregateSnapshot.Received(1).Save(this.testScheme);
            }

            [It(nameof(GetAggregate))]
            public async Task Should_not_save_AggregateSnapshot_if_Aggregate_IsNew(
                )
            {
                this.testScheme.IsNew.Returns(true);

                var result = await this.GetAggregate(this.testScheme.Id);

                await this.aggregateSnapshot.DidNotReceiveWithAnyArgs().Save(this.testScheme);
            }
        }

        public class HandleSpec : AggregateRequestHandlerSpec
        {
            private readonly SchemePublishRequest testRequest = Substitute.For<SchemePublishRequest>();
            private DomainResult dispatchEventResult = DomainResult.Ok;
            private int dispatchDomainEvents_CallCount = 0;

            protected override Task<Dictionary<DomainEvent<PublicSchemeId>, DomainResult>> DispatchDomainEvents(IEventSourced<PublicSchemeId> aggregate)
            {
                aggregate.Should().BeSameAs(this.testScheme);
                dispatchDomainEvents_CallCount++;
                return base.DispatchDomainEvents(aggregate);
            }

            public HandleSpec() : base()
            {
                this.testScheme.ReleaseEvents()
                    .Returns(this.testEvents);

                this.testRequest.PublisherId.Returns(new PublisherId());
                //this.testRequest.AggregateId.Returns(new PublicSchemeId());
                this.testRequest.AggregateId.Returns(x => this.testScheme.Id);

                this.eventsDispatcher.DispatchEvent(Arg.Any<SchemePublishedEvent>())
                    .Returns(x => dispatchEventResult);

                this.aggregatesConfig.Value
                    .Returns(this.testCacheConfig);

                this.memoryCache.CreateEntry(this.testScheme.Id)
                    .Returns(this.cacheEntry);

                this.eventsAccessor.Read(this.testScheme)
                    .Returns(new DomainEventsResult<PublicSchemeId>(this.testEvents));

                this.aggregateSnapshot.Read(this.testScheme.Id)
                    .Returns(new AggregateResult<PublicScheme, PublicSchemeId>(this.testScheme));
            }

            [It(nameof(Handle))]
            public async Task Should_return_Error_if_GetAggregate_returns_Error(
                )
            {
                var expectedResult = new AggregateResult<PublicScheme, PublicSchemeId>("error");

                this.aggregateSnapshot.Read(this.testScheme.Id)
                    .Returns(expectedResult);

                var result = await this.Handle(this.testRequest, new CancellationToken());

                result.Should().BeSameAs(expectedResult);
            }

            [It(nameof(Handle))]
            public async Task Should_call_HandleDomainRequest(
                )
            {
                var result = await this.Handle(this.testRequest, new CancellationToken());

                this.handleDomainRequest_CallCount.Should().Be(1);
            }

            [It(nameof(Handle))]
            public async Task Should_call_DispatchDomainEvents(
                )
            {
                var result = await this.Handle(this.testRequest, new CancellationToken());

                this.dispatchDomainEvents_CallCount.Should().Be(1);
            }

            [It(nameof(Handle))]
            public async Task Should_return_Error_from_DispatchDomainEvents(
                )
            {
                this.dispatchEventResult = DomainResult.UnknownError;

                var result = await this.Handle(this.testRequest, new CancellationToken());

                result.Should().BeSameAs(this.dispatchEventResult);
            }

            [It(nameof(Handle))]
            public async Task Should_remove_Aggregate_from_MemoryCache_if_failed_to_DispatchEvent(
                )
            {
                this.dispatchEventResult = DomainResult.UnknownError;

                var result = await this.Handle(this.testRequest, new CancellationToken());

                this.memoryCache.Received(1).Remove(this.testScheme.Id);
            }
        }
    }
}
