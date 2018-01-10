using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MidnightLizard.Schemes.Domain.Scheme;
using MidnightLizard.Schemes.Processor.Processors;

namespace MidnightLizard.Schemes.Processor.Controllers
{
    [Route("[controller]/[action]")]
    public class ProcessorsController : Controller
    {
        private readonly ICommandProcessor commandProcessor;
        private readonly ISchemesRepository fuck;

        public ProcessorsController(ICommandProcessor commandProcessor, ISchemesRepository fuck)
        {
            this.commandProcessor = commandProcessor;
            this.fuck = fuck;
        }

        [HttpPost]
        public void Start([FromBody]string value)
        {
            this.commandProcessor.Start();
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
