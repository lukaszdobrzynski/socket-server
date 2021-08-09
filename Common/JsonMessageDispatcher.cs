using System;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Common
{
    public class JsonMessageDispatcher : MessageDispatcher<JObject>
    {
        protected override string GetRouteFrom(MethodInfo methodInfo)
        {
            var attribute = methodInfo.GetCustomAttribute<RouteAttribute>();
            if (attribute == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(RouteAttribute)} is required to register the {methodInfo.Name} handler");
            }
            
            return attribute.Route;
        }

        protected override T ConvertFrom<T>(JObject messageType)
        {
            return messageType.ToObject<T>();
        }

        protected override JObject ConvertFrom<T>(T message)
        {
            return JObject.FromObject(message);
        }

        protected override string GetRouteFrom(JObject message)
        {
            message.TryGetValue(nameof(RouteAttribute.Route), StringComparison.OrdinalIgnoreCase, out var token);

            return token?.Value<string>();
        }
    }
}