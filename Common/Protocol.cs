using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Common
{
    public abstract class Protocol
    {
        private const int HeaderLengthInBytes = 4;

        private async Task<byte[]> Read(NetworkStream stream, int bytesToRead)
        {
            var buffer = new byte[bytesToRead];
            var bytesRead = 0;

            while (bytesRead < bytesToRead)
            {
                var bytesReceived = await stream.ReadAsync(buffer, bytesRead, (bytesToRead - bytesRead));

                if (bytesReceived == 0)
                    throw new Exception("Socket closed!");

                bytesRead += bytesReceived;
            }

            return buffer;
        }

        private byte[] Encode<T>(T message)
        {
            var bodyBytes = EncodeBody(message);
            var headerBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bodyBytes.Length));
            
            var headerAndBody = new byte[headerBytes.Length + bodyBytes.Length];
            
            System.Buffer.BlockCopy(headerBytes, 0, headerAndBody, 0, headerBytes.Length);
            System.Buffer.BlockCopy(bodyBytes, 0, headerAndBody, headerBytes.Length, bodyBytes.Length);

            return headerAndBody;
        }

        private async Task<int> ReadHeader(NetworkStream stream)
        {
            var headerBytes = await Read(stream, HeaderLengthInBytes);
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerBytes));
        }

        private async Task<T> ReadBody<T>(NetworkStream stream, int bodyLength)
        {
            var bodyBytes = await Read(stream, bodyLength);
            return Decode<T>(bodyBytes);
        }

        public async Task<T> Receive<T>(NetworkStream stream)
        {
            var bodyLength = await ReadHeader(stream);
            var body = await ReadBody<T>(stream, bodyLength);
            return body;
        }

        public async Task Send<T>(NetworkStream stream, T message)
        {
            var headerAndBody = Encode(message);
            await stream.WriteAsync(headerAndBody);
        }

        protected abstract T Decode<T>(byte[] bytes);
        
        protected abstract byte[] EncodeBody<T>(T message);
    }
}