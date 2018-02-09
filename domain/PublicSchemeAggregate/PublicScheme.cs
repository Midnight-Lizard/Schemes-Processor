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
        public PublisherId PublisherId { get; private set; }
        public ColorScheme ColorScheme { get; private set; }

        public PublicScheme() { }
        public PublicScheme(bool isNew) : base(isNew) { }

        public virtual void Publish(PublisherId publisherId, Guid correlationId, IColorScheme colorScheme)
        {
            if (IsNew || PublisherId == publisherId)
            {
                // var validatedColorScheme = 
                if (IsNew || !colorScheme.Equals(ColorScheme))
                {
                    PublisherId = publisherId;


                    IsNew = false;
                }
            }
            else if (PublisherId != publisherId)
            {
                // access denied event
            }
        }
    }
}
