using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MidnightLizard.Schemes.Processor.Application.DomainRequestHandlers
{
    public class AggregateRequestHandlerSpec
    {
        [Fact(DisplayName = nameof(ShouldDoSomething))]
        public void ShouldDoSomething()
        {
            Assert.Equal("fuck", "fuck");
        }
    }
}
