using MidnightLizard.Commons.Domain.Model;
using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public partial class PublicScheme : AggregateRoot<PublicSchemeId>
    {
        public override void Reduce(DomainEvent<PublicSchemeId> @event, UserId publisherId)
        {
            switch (@event)
            {
                case SchemePublishedEvent published:
                    this.PublisherId = publisherId;
                    this.ColorScheme = published.ColorScheme;
                    break;

                default:
                    break;
            }
        }
    }
}
