using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Common
{
    public class ClientChannel<TProtocol, TMessageType> : Channel<TProtocol, TMessageType> 
        where TProtocol : Protocol, new()
    {
        public async Task Connect(IPEndPoint endpoint)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(endpoint);
            Attach(socket);
        }
    }
}