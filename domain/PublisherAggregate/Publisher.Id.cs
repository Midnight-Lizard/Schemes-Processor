using MidnightLizard.Commons.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublisherAggregate
{
    public class PublisherId : DomainEntityId<string>
    {
        public static DomainEntityIdValidator<string> Validator = new DomainEntityIdValidator<string>();

        protected PublisherId() { }

        public PublisherId(string id) : base(id) { }
    }
}
