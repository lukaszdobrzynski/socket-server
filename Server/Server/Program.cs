using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common;
using Newtonsoft.Json.Linq;

namespace Server
{
    class Program
    {
        private static JsonMessageDispatcher _messageDispatcher = new JsonMessageDispatcher();
        static void Main(string[] args)
        {
            _messageDispatcher.Register<Message, Message>(ReportHeartbeat);
            
            var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endpoint);
            socket.Listen(128);

            _ = Task.Run(() => HandleConnectionQueue(socket));

            Console.ReadLine();
        }

        private static async Task HandleConnectionQueue(Socket socket)
        {
            do
            {
                var clientSocket = await Task.Factory
                    .FromAsync(socket.BeginAccept, socket.EndAccept, null);
                
                Console.WriteLine("Socket Server :: Client Connected");
                
                var channel = new Channel<JsonProtocol, JObject>();
                channel.Attach(clientSocket);
                _messageDispatcher.Bind(channel);
                
            } while (true);
        }

        [Route("api/heartbeat")]
        private static Task<Message> ReportHeartbeat(Message message)
        {
            return Task.FromResult(message);
        }
    }
}