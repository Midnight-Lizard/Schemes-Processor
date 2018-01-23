using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace MidnightLizard.Schemes.Processor.Controllers
{
    [Route("[controller]/[action]")]
    public class StatusController : Controller
    {
        private readonly IApplicationLifetime appLifetime;

        public StatusController(IApplicationLifetime appLifetime)
        {
            this.appLifetime = appLifetime;
        }

        public IActionResult IsReady()
        {
            return Ok("schemes processor is ready");
        }

        public IActionResult IsAlive()
        {
            return Ok("schemes processor is alive");
        }

        [HttpPost]
        public IActionResult Stop()
        {
            appLifetime.StopApplication();
            return Ok("schemes processor is ready to be stopped");
        }
    }
}
