using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Versioning
{
    public class AppVersion
    {
        public AppVersion(string version)
        {
            Value = new SemVer.Version(version);
        }

        public virtual SemVer.Version Value { get; private set; }

        public override string ToString()
        {
            return Value?.ToString();
        }

        public static AppVersion Latest { get; } = new AppVersion("1.3.0");
    }
}
