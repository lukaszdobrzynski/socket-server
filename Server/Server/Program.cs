using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common;

namespace Server
{
    class Program
    {
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

                var networkStream = new NetworkStream(clientSocket);
                var protocol = new JsonProtocol();
                var message = await protocol.Receive<Message>(networkStream);
                await protocol.Send(networkStream, message);

            } while (true);
        }
    }
}