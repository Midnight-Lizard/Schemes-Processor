using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Features.Variance;
using System.Threading.Tasks;
using System.Reflection;
using MidnightLizard.Schemes.Infrastructure.Queue;
using MediatR;
using MidnightLizard.Schemes.Domain.Common.Results;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common;

namespace MidnightLizard.Schemes.Infrastructure.AutofacModules
{
    public class MessageSerializationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var thisAssambly = typeof(MessageSerializationModule).GetTypeInfo().Assembly;
            builder.RegisterAssemblyTypes(thisAssambly)
                .AsClosedTypesOf(typeof(IMessageDeserializer<>))
                .Keyed<IMessageDeserializer>(t =>
                {
                    return t.GetInterfaces().First().GetGenericArguments()[0].Name +
                     t.GetCustomAttribute<MessageVersionAttribute>().Version.ToString();
                });
            builder.RegisterType<MessageSerializer>().AsSelf().SingleInstance();

            builder.RegisterAssemblyTypes(thisAssambly).AsImplementedInterfaces();
        }
    }
}
