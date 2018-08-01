using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private static ThreadLocal<AutofacServiceProvider> autofacServiceProvider = new ThreadLocal<AutofacServiceProvider>();

        public StartupStub(IConfiguration configuration) : base(configuration)
        {
        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var container = base.ConfigureServices(services) as AutofacServiceProvider;

            autofacServiceProvider.Value = container;

            return container;
        }

        public static TResult Resolve<TResult>()
        {
            using (new TestServer(new WebHostBuilder().UseStartup<StartupStub>())) { }

            return autofacServiceProvider.Value.GetService<TResult>();
        }

        public static TResult Resolve<TResult, TService>(Func<IServiceProvider, TService> withServiceFactory)
            where TService : class
        {
            using (new TestServer(new WebHostBuilder().UseStartup<StartupStub>()
                .ConfigureServices(services =>
                {
                    services.AddScoped<TService>(withServiceFactory);
                }))) { }

            return autofacServiceProvider.Value.GetService<TResult>();
        }
    }
}
