using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Press ENTER to connect...");
            Console.ReadLine();
            
            var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            await socket.ConnectAsync(endpoint);

            var message = new Message
            {
                Payload = "Hello from Echo Server!"
            };

            var networkStream = new NetworkStream(socket);

            var protocol = new JsonProtocol();
            await protocol.Send(networkStream, message);

            var response = await protocol.Receive<Message>(networkStream);

            Console.WriteLine($"Message from echo server: {response.Payload}");

        }
    }
}