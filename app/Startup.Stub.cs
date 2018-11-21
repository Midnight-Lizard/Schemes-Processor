using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace MidnightLizard.Schemes.Processor
{
    public class StartupStub : Startup
    {
        public StartupStub(IConfiguration configuration) : base(configuration)
        {
        }

        public static TResult Resolve<TResult>()
        {
            return new WebHostBuilder().UseStartup<StartupStub>()
                .Build().Services.GetService<TResult>();
        }

        public static TResult Resolve<TResult, TService>(Func<IServiceProvider, TService> withServiceFactory)
            where TService : class
        {
            return new WebHostBuilder().UseStartup<StartupStub>()
                .ConfigureServices(services =>
                {
                    services.AddScoped<TService>(withServiceFactory);
                }).Build().Services.GetService<TResult>();
        }
    }
}
