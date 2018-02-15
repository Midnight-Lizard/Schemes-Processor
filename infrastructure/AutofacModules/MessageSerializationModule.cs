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
            builder.RegisterType<MessageSerializer>()
                .As<IMessageSerializer>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(MessageSerializationModule).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IMessageDeserializer<>))
                .Keyed<IMessageDeserializer>(t =>
                {
                    var msgAttr = t.GetCustomAttribute<MessageAttribute>();
                    var msgType = msgAttr.Type ?? t.GetInterfaces().First().GetGenericArguments()[0].Name;
                    return $"{msgType}{msgAttr.Version}";
                });
        }
    }
}
