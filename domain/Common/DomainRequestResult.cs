using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class DomainRequestResult
    {
        public bool HasError { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public Exception Exception { get; protected set; }
    }
}
