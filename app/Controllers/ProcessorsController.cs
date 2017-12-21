using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MidnightLizard.Schemes.Processor.Processors;

namespace MidnightLizard.Schemes.Processor.Controllers
{
    [Route("[controller]/[action]")]
    public class ProcessorsController : Controller
    {
        private readonly ICommandProcessor commandProcessor;

        public ProcessorsController(ICommandProcessor commandProcessor)
        {
            this.commandProcessor = commandProcessor;
        }

        [HttpPost]
        public void Start([FromBody]string value)
        {
            this.commandProcessor.Start();
        }
    }
}
