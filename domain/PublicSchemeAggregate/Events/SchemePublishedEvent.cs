using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events
{
    public class SchemePublishedEvent : SchemeDomainEvent
    {
        public override Version LatestVersion() => new Version(1, 3);

        public PublisherId PublisherId { get; private set; }

        public ColorScheme ColorScheme { get; private set; }

        protected SchemePublishedEvent() { }

        public SchemePublishedEvent(PublicSchemeId aggregateId, PublisherId publisherId, ColorScheme colorScheme)
            : base(aggregateId)
        {
            ColorScheme = colorScheme;
            PublisherId = publisherId;
        }
    }
}
