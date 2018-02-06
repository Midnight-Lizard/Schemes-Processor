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
