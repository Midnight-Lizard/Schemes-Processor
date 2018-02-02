using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public class PublicScheme : AggregateRoot<PublicSchemeId>
    {
        public PublisherId PublisherId { get; protected set; }
        public ColorScheme ColorScheme { get; protected set; }

        public void Publish(
            PublisherId publisherId,
            Guid correlationId,
            ColorScheme colorScheme)
        {
            if (PublisherId.IsDefault || PublisherId == publisherId)
            {
                if (!colorScheme.Equals(ColorScheme))
                {

                }
            }
            else
            {

            }
        }
    }
}
