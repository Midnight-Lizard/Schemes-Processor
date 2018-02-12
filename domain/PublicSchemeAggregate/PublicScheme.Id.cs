﻿using MidnightLizard.Schemes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public class PublicSchemeId : DomainEntityId<Guid>
    {
        public PublicSchemeId() : base()
        {
        }

        public PublicSchemeId(Guid value) : base(value)
        {
        }
    }
}
