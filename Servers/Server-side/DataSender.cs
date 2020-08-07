using System.Net;

namespace Server
{
    public enum ServerPackets
    {
        SWelcomeMessage = 1,
    }

    class DataSender
    {
        public static void SendData(int[] targets, byte[] data)
        {
            foreach (int target in targets)
            {
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteInteger((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
                buffer.WriteBytes(data);
                Server.clients[target].stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
                buffer.Dispose();
            }
        }

        #region Packet methods
        public static void SendWelcomeMessage(int fromID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInteger((int)ServerPackets.SWelcomeMessage);
            buffer.WriteString($"{((IPEndPoint)Server.clients[fromID].socket.Client.RemoteEndPoint).Address.ToString()} joined the server!");
            SendData(Server.GetClientsID(new int[]{fromID}), buffer.ToArray());
            buffer.Dispose();
        }

        #endregion
    }
}
