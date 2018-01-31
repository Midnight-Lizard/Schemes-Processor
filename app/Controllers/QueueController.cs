using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Infrastructure;

namespace MidnightLizard.Schemes.Processor.Controllers
{
    [Route("[controller]/[action]")]
    public class QueueController : Controller
    {
        private readonly ISchemesQueue queue;
        private readonly IApplicationLifetime appLifetime;

        public QueueController(ISchemesQueue schemesQueue, IApplicationLifetime appLifetime)
        {
            this.queue = schemesQueue;
            this.appLifetime = appLifetime;
        }

        [HttpPost]
        public async Task Start()
        {
            await this.queue.BeginProcessing(appLifetime.ApplicationStopping);
        }

        [HttpPost]
        public async void Pause()
        {
            await this.queue.Pause();
        }

        [HttpPost]
        public async void Resume()
        {
            await this.queue.Resume(appLifetime.ApplicationStopping);
        }
    }
}
