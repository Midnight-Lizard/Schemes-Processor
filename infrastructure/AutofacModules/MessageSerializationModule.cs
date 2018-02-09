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
using MidnightLizard.Schemes.Infrastructure.Serialization;

namespace MidnightLizard.Schemes.Infrastructure.AutofacModules
{
    public class MessageSerializationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(MessageSerializationModule).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IMessageDeserializer<>))
                .Keyed<IMessageDeserializer<BaseMessage>>(t =>
                    t.GetGenericArguments()[0].Name +
                    t.GetCustomAttribute<MessageVersionAttribute>().Version.ToString(2));
        }
    }
}
