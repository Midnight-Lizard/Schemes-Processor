using FluentValidation.Results;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Results;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public partial class PublicScheme : AggregateRoot<PublicSchemeId>
    {
        public UserId PublisherId { get; private set; }
        public ColorScheme ColorScheme { get; private set; }

        public PublicScheme() { }
        public PublicScheme(PublicSchemeId publicSchemeId) : base(publicSchemeId) { }

        public virtual void Publish(UserId publisherId, ColorScheme colorScheme)
        {
            var publisherIdValidationResults = new DomainEntityIdValidator<string>().Validate(publisherId);
            if (publisherIdValidationResults.IsValid)
            {
                if (this.IsNew() || this.PublisherId == publisherId)
                {
                    var colorSchemeValidationResults = ColorScheme.Validator.Validate(colorScheme);
                    if (colorSchemeValidationResults.IsValid)
                    {
                        if (this.IsNew() || colorScheme != this.ColorScheme)
                        {
                            AddSchemePublishedEvent(publisherId, colorScheme);
                        }
                    }
                    else
                    {
                        AddColorSchemeValidationFailedEvent(publisherId, colorSchemeValidationResults);
                    }
                }
                else if (this.PublisherId != publisherId)
                {
                    AddPublisherAccessDeniedEvent(publisherId);
                }
            }
            else
            {
                AddPublisherIdValidationFailedEvent(publisherId, publisherIdValidationResults);
            }
        }

        private void AddPublisherIdValidationFailedEvent(UserId publisherId, ValidationResult publisherIdValidationResults)
        {
            this.AddDomainEvent(new PublisherIdValidationFailedEvent(this.Id, publisherIdValidationResults), publisherId);
        }

        private void AddPublisherAccessDeniedEvent(UserId publisherId)
        {
            this.AddDomainEvent(new SchemeAccessDeniedEvent(this.Id), publisherId);
        }

        private void AddColorSchemeValidationFailedEvent(UserId publisherId, ValidationResult validationResults)
        {
            this.AddDomainEvent(new ColorSchemeValidationFailedEvent(this.Id, validationResults), publisherId);
        }

        private void AddSchemePublishedEvent(UserId publisherId, ColorScheme colorScheme)
        {
            this.AddDomainEvent(new SchemePublishedEvent(this.Id, colorScheme), publisherId);
        }
    }
}
