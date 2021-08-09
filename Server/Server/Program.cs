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
        private static Channel<JsonProtocol, JObject> _channel = new Channel<JsonProtocol, JObject>();
        
        static void Main(string[] args)
        {
            
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

                _channel.Attach(clientSocket);
                _channel.OnMessage( async (jObject) => await _channel.Send(jObject));
                
            } while (true);
        }
    }
}