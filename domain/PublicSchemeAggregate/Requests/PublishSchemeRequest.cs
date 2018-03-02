using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Requests
{
    public class PublishSchemeRequest : SchemeDomainRequest
    {
        public virtual PublisherId PublisherId { get; set; }

        public ColorScheme ColorScheme { get; set; }
    }
}
