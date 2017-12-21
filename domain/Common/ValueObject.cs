using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public abstract class ValueObject
    {
        public override bool Equals(object other)
        {
            return other != null &&
                other is ValueObject vo &&
                vo.GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
