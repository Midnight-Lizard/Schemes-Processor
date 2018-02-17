using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Queue
{
    public class PublicSchemeEventDispatcher : DomainEventDispatcher<PublicSchemeId>
    {
        public PublicSchemeEventDispatcher(KafkaConfig kafkaConfig, IMessageSerializer messageSerializer)
            : base(kafkaConfig, messageSerializer)
        {
        }

        protected override string GetEventTopicName() => this.kafkaConfig.SCHEMES_EVENTS_TOPIC;
    }
}
