using System;
using System.Collections.Generic;
using UnityEngine;

static class DataReceiver
{
    public static ByteBuffer playerBuffer;

    public delegate void Packet(byte[] data);
    public static Dictionary<int, Packet> packets = new Dictionary<int, Packet>();

    public static void Initialize()
    {
        packets.Add((int)Packets.SInit, ConnectionInitailize);
        packets.Add((int)Packets.CEcho, HandleEcho);
        packets.Add((int)Packets.CInstantiate, HandleInstantiate);
        packets.Add((int)Packets.CUpdateTransform, HandleUpdateTransform);
        packets.Add((int)Packets.CTakeDamages, HandleTakeDamages);
        packets.Add((int)Packets.CDie, HandleOtherDie);
    }


    public static void HandleData(byte[] data)
    {
        byte[] buffer = (byte[])data.Clone();
        int packetLength = 0;

        if (playerBuffer == null)
        {
            playerBuffer = new ByteBuffer();
        }

        playerBuffer.WriteBytes(buffer);
        if (playerBuffer.Count() == 0)
        {
            playerBuffer.Clear();
            return;
        }

        if (playerBuffer.Length() >= 4)
        {
            packetLength = playerBuffer.ReadInteger(false);
            if (packetLength <= 0)
            {
                playerBuffer.Clear();
                return;
            }
        }

        while (packetLength > 0 & packetLength <= playerBuffer.Length() - 4)
        {
            if (packetLength <= playerBuffer.Length() - 4)
            {
                playerBuffer.ReadInteger();
                data = playerBuffer.ReadBytes(packetLength);

                ByteBuffer byteBuffer = new ByteBuffer();
                byteBuffer.WriteBytes(data);
                int packetID = byteBuffer.ReadInteger();
                byteBuffer.Dispose();
                if (packets.TryGetValue(packetID, out Packet packet))
                {
                    packet.Invoke(data);
                }
            }

            packetLength = 0;
            if (playerBuffer.Length() >= 4)
            {
                packetLength = playerBuffer.ReadInteger(false);
                if (packetLength <= 0)
                {
                    playerBuffer.Clear();
                    return;
                }
            }
        }

        if (packetLength <= 1)
        {
            playerBuffer.Clear();
        }
    }


    #region Packet methods
    private static void ConnectionInitailize (byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger();
        int connectionID = buffer.ReadInteger();
        buffer.Dispose();

        Client.connectionID = connectionID;
        Debug.Log($"Server initialization packets received! Your connection id is : {Client.connectionID}");
        NetworkManager.instance.CreateLocalPlayer();
    }

    private static void HandleEcho (byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger();
        int fromID = buffer.ReadInteger();
        string to = buffer.ReadString();
        string echoMessage = buffer.ReadString();
        buffer.Dispose();
        Debug.Log($"Echo from {fromID} : {echoMessage}");
    }

    private static void HandleInstantiate (byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger();
        int fromID = buffer.ReadInteger();
        buffer.ReadString();
        string stringPos = buffer.ReadString();
        bool calledForResponse = buffer.ReadBool();
        buffer.Dispose();

        string[] components = stringPos.Split(';');
        Vector3 pos = new Vector3(float.Parse(components[0]), float.Parse(components[1]), float.Parse(components[2]));
        NetworkManager.instance.CreateNetworkPlayer(fromID, pos);

        if (!calledForResponse)
        {
            DataSender.SendInstantiate(NetworkManager.GetLocalPlayer().transform.position, $"to {fromID}", true);
        }
    }

    private static void HandleUpdateTransform (byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger();
        int fromID = buffer.ReadInteger();
        buffer.ReadString();
        string stringTransform = buffer.ReadString();
        buffer.Dispose();

        string[] components = stringTransform.Split(';');
        Vector3 pos = new Vector3(float.Parse(components[0]), float.Parse(components[1]), float.Parse(components[2]));
        float zRot = float.Parse(components[3]);
        NetworkManager.instance.UpdateNetworkPlayerTransform(fromID, pos, zRot);
    }

    private static void HandleTakeDamages (byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger();
        int fromID = buffer.ReadInteger();
        buffer.ReadString();
        float damages = buffer.ReadFloat();
        buffer.Dispose();

        NetworkManager.GetLocalPlayer().GetComponent<Player>().TakeDamages(damages, fromID);
    }

    private static void HandleOtherDie (byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(data);
        buffer.ReadInteger();
        int fromID = buffer.ReadInteger();
        buffer.ReadString();
        int killerID = buffer.ReadInteger();
        buffer.Dispose();

        NetworkManager.DestroyGameObject(NetworkManager.GetNetworkPlayer(fromID));
    }
    #endregion
}
