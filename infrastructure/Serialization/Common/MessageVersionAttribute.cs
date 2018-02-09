using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization
{
    public class MessageVersionAttribute : Attribute
    {
        public Version Version { get; }

        public MessageVersionAttribute(string version)
        {
            Version = Version.Parse(version);
        }

    }
}
