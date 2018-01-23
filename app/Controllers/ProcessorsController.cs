using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MidnightLizard.Schemes.Domain.Scheme;
using MidnightLizard.Schemes.Processor.Processors;

namespace MidnightLizard.Schemes.Processor.Controllers
{
    [Route("[controller]/[action]")]
    public class ProcessorsController : Controller
    {
        private readonly ISchemesProcessor commandProcessor;
        private readonly ISchemesRepository fuck;
        private readonly IApplicationLifetime appLifetime;

        public ProcessorsController(
            ISchemesProcessor commandProcessor,
            ISchemesRepository fuck,
            IApplicationLifetime appLifetime)
        {
            this.commandProcessor = commandProcessor;
            this.fuck = fuck;
            this.appLifetime = appLifetime;
        }

        [HttpPost]
        public async Task Start()
        {
            await this.commandProcessor.Start(appLifetime.ApplicationStopping);
        }

        [HttpPost]
        public void Pause()
        {
            this.commandProcessor.Pause();
        }

        [HttpPost]
        public IActionResult Save([FromBody]SchemeAggregateRoot scheme)
        {
            if (ModelState.IsValid)
            {
                this.fuck.Save(scheme);
                return Ok();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
