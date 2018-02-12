using FluentValidation.Results;
using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
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
        public PublicScheme(PublicSchemeId publicSchemeId) : base(publicSchemeId) { }

        public virtual void Publish(PublisherId publisherId, ColorScheme colorScheme)
        {
            if (this.IsNew() || this.PublisherId == publisherId)
            {
                var validationResults = ColorScheme.Validator.Validate(colorScheme);
                if (validationResults.IsValid)
                {
                    if (this.IsNew() || !colorScheme.Equals(this.ColorScheme))
                    {
                        AddSchemePublishedEvent(publisherId, colorScheme);
                    }
                }
                else
                {
                    AddColorSchemeValidationFailedEvent(validationResults);
                }
            }
            else if (this.PublisherId != publisherId)
            {
                // TODO: access denied event
            }
        }

        private void AddColorSchemeValidationFailedEvent(ValidationResult validationResults)
        {
            this.AddDomainEvent(new ColorSchemeValidationFailedEvent(this.Id, validationResults));
        }

        private void AddSchemePublishedEvent(PublisherId publisherId, ColorScheme colorScheme)
        {
            this.AddDomainEvent(new SchemePublishedEvent(this.Id, publisherId, colorScheme));
        }
    }
}
