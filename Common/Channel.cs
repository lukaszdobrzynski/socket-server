using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class Channel<TProtocol, TMessageType> where TProtocol : Protocol, new()
    {
        private NetworkStream _networkStream;
        private Task _receive;
        private Func<TMessageType, Task> _receiveCallback;
        private readonly TProtocol _protocol = new TProtocol();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void Attach(Socket clientSocket)
        {
            _networkStream = new NetworkStream(clientSocket);
            _receive = Task.Run(ReceiveLoop, _cancellationTokenSource.Token);
        }

        public async Task Send(TMessageType message)
        {
            if (_networkStream == null)
            {
                throw new InvalidOperationException(
                    $"The channel has not been attached. Have you forgot to call {nameof(Attach)} on it?");
            }

            await _protocol.Send(_networkStream, message);
        }

        public void OnMessage(Func<TMessageType, Task> receiveCallback) => _receiveCallback = receiveCallback;
        
        private async Task ReceiveLoop()
        {
            while (_cancellationTokenSource.IsCancellationRequested == false)
            {
                var message = await _protocol.Receive<TMessageType>(_networkStream);
                await _receiveCallback(message);
            }
        }
    }
}