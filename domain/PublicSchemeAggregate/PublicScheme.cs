using FluentValidation.Results;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public partial class PublicScheme : AggregateRoot<PublicSchemeId>
    {
        public override Version LatestVersion() => new Version(1, 2);
        public PublisherId PublisherId { get; private set; }
        public ColorScheme ColorScheme { get; private set; }

        public PublicScheme() { }
        public PublicScheme(PublicSchemeId publicSchemeId) : base(publicSchemeId) { }

        public virtual void Publish(PublisherId publisherId, ColorScheme colorScheme)
        {
            var publisherIdValidationResults = PublisherId.Validator.Validate(publisherId);
            if (publisherIdValidationResults.IsValid)
            {
                if (this.IsNew() || this.PublisherId == publisherId)
                {
                    var colorSchemeValidationResults = ColorScheme.Validator.Validate(colorScheme);
                    if (colorSchemeValidationResults.IsValid)
                    {
                        if (this.IsNew() || !colorScheme.Equals(this.ColorScheme))
                        {
                            AddSchemePublishedEvent(publisherId, colorScheme);
                        }
                    }
                    else
                    {
                        AddColorSchemeValidationFailedEvent(colorSchemeValidationResults);
                    }
                }
                else if (this.PublisherId != publisherId)
                {
                    AddPublisherAccessDeniedEvent(publisherId);
                }
            }
            else
            {
                AddPublisherIdValidationFailedEvent(publisherIdValidationResults);
            }
        }

        private void AddPublisherIdValidationFailedEvent(ValidationResult publisherIdValidationResults)
        {
            this.AddDomainEvent(new PublisherIdValidationFailedEvent(this.Id, publisherIdValidationResults));
        }

        private void AddPublisherAccessDeniedEvent(PublisherId publisherId)
        {
            this.AddDomainEvent(new PublisherAccessDeniedEvent(this.Id, publisherId));
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
