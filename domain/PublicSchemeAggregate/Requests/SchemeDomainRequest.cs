using MidnightLizard.Schemes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public abstract class SchemeDomainRequest : DomainRequest<PublicSchemeId>
    {
        public SchemeDomainRequest() { }
        public SchemeDomainRequest(PublicSchemeId id) : base(id) { }
    }
}
