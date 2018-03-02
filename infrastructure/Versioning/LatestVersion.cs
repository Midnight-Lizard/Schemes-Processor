using MidnightLizard.Commons.Domain.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Versioning
{
    public static class Latest
    {
        public static DomainVersion Version { get; } = new DomainVersion("1.3.0");
    }
}
