using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Requests
{
    public abstract class SchemeDomainRequest : DomainRequest<PublicSchemeId>
    {
    }
}
