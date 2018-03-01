﻿using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Requests
{
    public abstract class SchemeDomainRequest : DomainRequest<PublicSchemeId>
    {
    }
}
