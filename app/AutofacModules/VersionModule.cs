using Autofac;
using MidnightLizard.Schemes.Infrastructure;
using MidnightLizard.Schemes.Infrastructure.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Processor.AutofacModules
{
    public class VersionModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(Latest.Version);
        }
    }
}
