using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MidnightLizard.Schemes.Processor
{
    public class StartupStub : Startup
    {
        private static AutofacServiceProvider autofacServiceProvider;

        public StartupStub(IConfiguration configuration) : base(configuration)
        {

        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var container = base.ConfigureServices(services) as AutofacServiceProvider;

            autofacServiceProvider = container;

            return container;
        }

        public static TResult Resolve<TResult>()
        {
            if (autofacServiceProvider == null)
            {
                using (new TestServer(new WebHostBuilder().UseStartup<StartupStub>())) { }
            }

            return autofacServiceProvider.GetService<TResult>();
        }
    }
}
