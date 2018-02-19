using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MidnightLizard.Schemes.Processor
{
    public class StartupStub : Startup
    {
        public static AutofacServiceProvider AutofacServiceProvider;

        public StartupStub(IConfiguration configuration) : base(configuration)
        {

        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var container = base.ConfigureServices(services) as AutofacServiceProvider;

            AutofacServiceProvider = container;

            return container;
        }
    }
}
