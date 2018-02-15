using Autofac;
using MidnightLizard.Schemes.Domain.Common.Interfaces;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Infrastructure.Queue;
using MidnightLizard.Schemes.Infrastructure.Snapshot;

namespace MidnightLizard.Schemes.Infrastructure.AutofacModules
{
    public class DomainInfrastructureModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessagingQueue>()
                .As<IMessagingQueue>()
                .SingleInstance();

            builder.RegisterType<SchemesSnapshot>()
                .As<IAggregateSnapshotAccessor<PublicScheme, PublicSchemeId>>()
                .SingleInstance();

            //builder.RegisterAssemblyTypes(typeof(DomainInfrastructureModule).GetTypeInfo().Assembly)
            //    .AsImplementedInterfaces();
        }
    }
}
