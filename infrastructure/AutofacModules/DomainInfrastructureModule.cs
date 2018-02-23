using Autofac;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Infrastructure.EventStore;
using MidnightLizard.Schemes.Infrastructure.Queue;
using MidnightLizard.Schemes.Infrastructure.Snapshot;
using System.Reflection;

namespace MidnightLizard.Schemes.Infrastructure.AutofacModules
{
    public class DomainInfrastructureModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(DomainEventDispatcher<>).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IDomainEventDispatcher<>))
                .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(DomainEventStore<>).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IDomainEventStore<>))
                .SingleInstance();

            builder.RegisterType<MessagingQueue>()
                .As<IMessagingQueue>()
                .SingleInstance();

            builder.RegisterType<SchemesSnapshotAccessor>()
                .As<IAggregateSnapshotAccessor<PublicScheme, PublicSchemeId>>()
                .SingleInstance();

            //builder.RegisterAssemblyTypes(typeof(DomainInfrastructureModule).GetTypeInfo().Assembly)
            //    .AsImplementedInterfaces();
        }
    }
}
