using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization
{
    public class MessageContractResolver : DefaultContractResolver
    {
        public static readonly MessageContractResolver Default = new MessageContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            property.Writable = property.Writable ||
                member is PropertyInfo propInfo && propInfo.GetSetMethod(true) != null;

            return property;
        }
    }
}