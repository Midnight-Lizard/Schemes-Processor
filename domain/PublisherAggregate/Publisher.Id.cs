using MidnightLizard.Schemes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublisherAggregate
{
    public class PublisherId : DomainEntityId<Guid>
    {
        public PublisherId() { }

        public PublisherId(Guid id) : base(id) { }
    }
}
