using MidnightLizard.Schemes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public class PublicSchemeId : EntityId<Guid>
    {
        public PublicSchemeId()
        {
            Value = Guid.NewGuid();
        }

        public PublicSchemeId(Guid value)
        {
            Value = value;
        }
    }
}
