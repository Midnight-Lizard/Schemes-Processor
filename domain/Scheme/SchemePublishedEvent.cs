using MidnightLizard.Schemes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Scheme
{
    public class SchemePublishedEvent : Event
    {
        public int PublisherId { get; private set; }
        public ColorSchemeValueObject ColorScheme { get; set; }
    }
}
