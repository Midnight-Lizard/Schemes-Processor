using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MidnightLizard.Commons.Domain.Interfaces;

namespace MidnightLizard.Schemes.Processor.Controllers
{
    [Route("[controller]/[action]")]
    public class StatusController : Controller
    {
        private readonly IMessagingQueue queue;

        public StatusController(IMessagingQueue schemesQueue)
        {
            this.queue = schemesQueue;
        }

        public IActionResult IsReady()
        {
            return Ok("schemes processor is ready");
        }

        public IActionResult IsAlive()
        {
            if (this.queue.CheckStatus())
            {
                return Ok("schemes processor is alive");
            }
            return BadRequest("schemes processor has too many errors and should be restarted");
        }
    }
}
