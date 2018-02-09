using MidnightLizard.Schemes.Domain.PublicSchemeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization
{
    [MessageVersion("1.0")]
    public class SchemePublishRequestDeserializer_v1_0 : SchemePublishRequestDeserializer_Latest
    {
        public override SchemePublishRequest ParseMessage(string msg)
        {
            var request = base.ParseMessage(msg);
            // version 1.0 does not contain button component

            return request;
        }
    }

    [MessageVersion("1.1")]
    public class SchemePublishRequestDeserializer_Latest : BaseMessageDeserializer<SchemePublishRequest>
    {
    }
}
