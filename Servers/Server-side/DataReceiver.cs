using System;
using System.Collections.Generic;

namespace Server
{
    public enum ClientPackets
    {
        CWelcomeMessage = 1,
    }

    class DataReceiver
    {
        public delegate void Packet(int fromID, byte[] data);
        public static Dictionary<int, Packet> packets = new Dictionary<int, Packet>();

        public static void InitializePacketsMethods()
        {
            packets.Add((int)ClientPackets.CWelcomeMessage, HandleWelcomeMessage);
        }

        public static void HandleData(int fromID, byte[] data)
        {
            byte[] buffer = (byte[])data.Clone();
            int packetLength = 0;

            if (Server.clients[fromID].buffer == null)
            {
                Server.clients[fromID].buffer = new ByteBuffer();
            }

            Server.clients[fromID].buffer.WriteBytes(buffer);
            if (Server.clients[fromID].buffer.Count() == 0)
            {
                Server.clients[fromID].buffer.Clear();
                return;
            }

            if (Server.clients[fromID].buffer.Length() >= 4)
            {
                packetLength = Server.clients[fromID].buffer.ReadInteger(false);
                if (packetLength <= 0)
                {
                    Server.clients[fromID].buffer.Clear();
                    return;
                }
            }

            while (packetLength > 0 & packetLength <= Server.clients[fromID].buffer.Length() - 4)
            {
                if (packetLength <= Server.clients[fromID].buffer.Length() - 4)
                {
                    Server.clients[fromID].buffer.ReadInteger();
                    data = Server.clients[fromID].buffer.ReadBytes(packetLength);

                    ByteBuffer byteBuffer = new ByteBuffer();
                    byteBuffer.WriteBytes(data);
                    int packetID = byteBuffer.ReadInteger();
                    byteBuffer.Dispose();
                    if (packets.TryGetValue(packetID, out Packet packet))
                    {
                        packet.Invoke(fromID, data);
                    }
                }

                packetLength = 0;
                if (Server.clients[fromID].buffer.Length() >= 4)
                {
                    packetLength = Server.clients[fromID].buffer.ReadInteger(false);
                    if (packetLength <= 0)
                    {
                        Server.clients[fromID].buffer.Clear();
                        return;
                    }
                }
            }

            if (packetLength <= 1)
            {
                Server.clients[fromID].buffer.Clear();
            }
        }


        #region Packets methods
        public static void HandleWelcomeMessage(int fromID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInteger();
            string message = buffer.ReadString();
            buffer.Dispose();

            Console.WriteLine(message);
        }

        #endregion
    }
}
