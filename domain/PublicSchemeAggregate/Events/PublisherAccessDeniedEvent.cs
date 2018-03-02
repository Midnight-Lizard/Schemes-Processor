﻿using MidnightLizard.Commons.Domain.Messaging;
using MidnightLizard.Schemes.Domain.PublisherAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Events
{
    public class PublisherAccessDeniedEvent : AccessDeniedEvent<PublicSchemeId>
    {
        public PublisherId PublisherId { get; protected set; }

        protected PublisherAccessDeniedEvent() { }

        public PublisherAccessDeniedEvent(PublicSchemeId aggregateId, PublisherId publisherId) : base(aggregateId)
        {
            PublisherId = publisherId;
        }
    }
}
