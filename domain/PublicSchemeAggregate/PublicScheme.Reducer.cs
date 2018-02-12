using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Domain.Common.Messaging;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public partial class PublicScheme : AggregateRoot<PublicSchemeId>
    {
        public override void Reduce(DomainEvent<PublicSchemeId> @event)
        {
            switch (@event)
            {
                case SchemePublishedEvent published:
                    this.PublisherId = published.PublisherId;
                    this.ColorScheme = published.ColorScheme;
                    break;

                default:
                    break;
            }
        }
    }
}
