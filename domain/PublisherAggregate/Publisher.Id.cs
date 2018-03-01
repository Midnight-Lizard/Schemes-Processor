using MidnightLizard.Commons.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublisherAggregate
{
    public class PublisherId : DomainEntityId<Guid>
    {
        public static DomainEntityIdValidator<Guid> Validator = new DomainEntityIdValidator<Guid>();

        public PublisherId() { }

        public PublisherId(Guid id) : base(id) { }
    }
}
