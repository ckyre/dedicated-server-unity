using System;
using System.Net.Sockets;

namespace Server
{
    class Client
    {
        public int id;
        public TcpClient socket;
        public NetworkStream stream;
        private byte[] receiveBuffer;

        public Client(TcpClient _socket, int _id, int bufferSize = 4096)
        {
            socket = _socket;
            id = _id;

            socket.SendBufferSize = bufferSize;
            socket.ReceiveBufferSize = bufferSize;
            stream = socket.GetStream();
            receiveBuffer = new byte[bufferSize];
            stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, OnReceiveData, null);
        }

        private void OnReceiveData (IAsyncResult result)
        {
            try
            {
                int length = stream.EndRead(result);
                if (length <= 0)
                {
                    Server.OnClientDisconnect(this);
                    return;
                }

                Server.OnReceiveData(receiveBuffer);
                stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, OnReceiveData, null);
            }
            catch (Exception e)
            {
                Debug.ErrorMessage(e.ToString());
                Server.OnClientDisconnect(this);
                return;
            }
        }
    }
}
