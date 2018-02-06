using AutoMapper;
using MidnightLizard.Schemes.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate
{
    public partial class PublicScheme : AggregateRoot<PublicSchemeId>
    {
        public override void Reduce(DomainEvent<PublicSchemeId> @event, IMapper mapper)
        {
            switch (@event)
            {
                case SchemePublishedEvent published:
                    this.PublisherId = new PublisherAggregate.PublisherId(published.PublisherId);
                    this.ColorScheme = mapper.Map<IColorScheme, ColorScheme>(published);
                    break;

                default:
                    break;
            }
        }
    }
}
