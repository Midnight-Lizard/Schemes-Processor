using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure
{
    public class DomainVersion
    {
        public static DomainVersion Latest { get; } = new DomainVersion();

        public virtual SemVer.Version Value { get; private set; } = new SemVer.Version("1.3.0");

        public override string ToString()
        {
            return Value?.ToString();
        }
    }
}
