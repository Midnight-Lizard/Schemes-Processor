using MidnightLizard.Schemes.Domain.Common;
using MidnightLizard.Schemes.Infrastructure.Serialization.Common.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Common
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

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            if (properties != null)
            {
                return properties.OrderBy(p => GetTypeLevel(p.DeclaringType)).ToList();
            }
            return properties;
        }

        private int GetTypeLevel(Type type)
        {
            if (typeLevels.ContainsKey(type))
            {
                return typeLevels[type];
            }
            else
            {
                var level = 0;
                var baseType = type.BaseType;
                while (baseType != null && baseType.Namespace != null &&
                    baseType.Namespace.StartsWith(nameof(MidnightLizard)))
                {
                    baseType = baseType.BaseType;
                    level++;
                }
                typeLevels.Add(type, level);
                return level;
            }
        }

        private static readonly Dictionary<Type, int> typeLevels = new Dictionary<Type, int>();
    }
}