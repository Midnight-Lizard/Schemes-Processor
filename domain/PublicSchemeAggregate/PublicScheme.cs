using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Results;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public partial class PublicScheme : AggregateRoot<PublicSchemeId>
    {
        public override Version Version() => new Version(1, 2);
        public PublisherId PublisherId { get; private set; }
        public ColorScheme ColorScheme { get; private set; }

        public PublicScheme() { }
        public PublicScheme(bool isNew) : base(isNew) { }

        public virtual void Publish(PublisherId publisherId, ColorScheme colorScheme)
        {
            if (this.IsNew() || this.PublisherId == publisherId)
            {
                // var validatedColorScheme = 
                if (this.IsNew() || !colorScheme.Equals(this.ColorScheme))
                {
                    AddSchemePublishedEvent(colorScheme);

                    this.isNew = false;
                }
            }
            else if (this.PublisherId != publisherId)
            {
                // access denied event
            }
        }

        private void AddSchemePublishedEvent(ColorScheme colorScheme)
        {
            this.AddDomainEvent(new SchemePublishedEvent(this.Id, this.PublisherId, colorScheme));
        }
    }
}
