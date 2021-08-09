using System;
using System.Net;
using System.Threading.Tasks;
using Common;
using Newtonsoft.Json.Linq;

namespace Client
{
    class Program
    {
        private static ClientChannel<JsonProtocol, JObject> _channel = new ClientChannel<JsonProtocol, JObject>();
        static async Task Main(string[] args)
        {
            Console.WriteLine("Press ENTER to connect...");
            Console.ReadLine();
            
            var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
            await _channel.Connect(endpoint);
            _channel.OnMessage((jObject) =>
            {
                var response = jObject.ToObject<Message>();
                Console.WriteLine($"Message from echo server: {response.Payload}");
                return Task.CompletedTask;
            });

            var message = new Message
            {
                Payload = "Hello from Echo Server!",
                Route = "api/heartbeat"
            };

            while (true)
            {
                await _channel.Send(JObject.FromObject(message));
                await Task.Delay(3000);
            }
            
            Console.ReadLine();
        }
    }
}