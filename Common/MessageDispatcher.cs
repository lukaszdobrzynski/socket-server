using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Common
{
    public abstract class MessageDispatcher<TMessageType> 
        where TMessageType : class
    {
        private readonly Dictionary<string, Func<TMessageType, Task<TMessageType>>>
            _handlers = new Dictionary<string, Func<TMessageType, Task<TMessageType>>>();

        public void Register<TParam, TResult>(Func<TParam, Task<TResult>> target)
        {
            var route = GetRouteFrom(target.GetMethodInfo());

            var handler = new Func<TMessageType, Task<TMessageType>>(async messageType =>
            {
                var param = ConvertFrom<TParam>(messageType);
                var result = await target(param);
                return ConvertFrom(result);
            });
            
            _handlers.Add(route, handler);
        }

        public void Bind<TProtocol>(Channel<TProtocol, TMessageType> channel) where TProtocol : Protocol, new()
        {
            channel.OnMessage(async message =>
            {
                var response = await Dispatch(message);
                await channel.Send(response);
            });        
        }

        private async Task<TMessageType> Dispatch(TMessageType message)
        {
            var route = GetRouteFrom(message);

            if (route == null)
            {
                //TODO return meaningful message
                return null;
            }

            _handlers.TryGetValue(route, out var handler);

            if (handler == null)
            {
                //TODO return meaningful message
                return null;
            }

            return await handler(message);
        }

        protected abstract string GetRouteFrom(MethodInfo methodInfo);
        protected abstract T ConvertFrom<T>(TMessageType messageType);
        protected abstract TMessageType ConvertFrom<T>(T message);
        protected abstract string GetRouteFrom(TMessageType message);
    }
}