using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Publisher;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Scheme
{
    public class SchemeAggregateRoot : AggregateRoot<Guid>
    {
        public PublisherId PublisherId { get; set; }
        public ColorSchemeValueObject ColorScheme { get; set; }

        public void Publish(
            PublisherId publisherId,
            Guid correlationId,
            ColorSchemeValueObject colorScheme)
        {
            if (publisherId == PublisherId)
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
