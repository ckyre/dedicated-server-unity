using System;
using System.Linq;
using System.Net.Sockets;

namespace Server
{
    public class Client
    {
        public int connexionID;
        public TcpClient socket;
        public NetworkStream stream;
        private byte[] receiveBuffer;
        public ByteBuffer buffer;

        public void Initialize()
        {
            socket.SendBufferSize = 4096;
            socket.ReceiveBufferSize = 4096;
            stream = socket.GetStream();
            receiveBuffer = new byte[4096];
            stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, OnReceiveData, null);

            Console.WriteLine($"Incoming connection from {socket.Client.RemoteEndPoint.ToString()}.");
        }

        private void OnReceiveData(IAsyncResult result)
        {
            try
            {
                int length = stream.EndRead(result);
                if (length <= 0)
                {
                    CloseConnection();
                    return;
                }

                byte[] newBytes = new byte[length];
                Array.Copy(receiveBuffer, newBytes, length);
                DataReceiver.HandleData(connexionID, newBytes);

                stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, OnReceiveData, null);
                buffer.Clear();
            }
            catch (Exception)
            {
                CloseConnection();
                return;
            }
        }

        private void CloseConnection()
        {
            Console.WriteLine($"{socket.Client.RemoteEndPoint.ToString()} has left the server.");
            Server.clients.Remove(connexionID);
            socket.Close();
        }
    }
}
