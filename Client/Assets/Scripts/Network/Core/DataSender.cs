using UnityEngine;
using System;

public enum Packets
{
    SInit = 1,
    CEcho = 2,
    CInstantiate = 3,
    CUpdateTransform = 4,
    CTakeDamages = 5,
    CDie = 6,
}

//Packets format : int packetLength (jamais utlisé), int methodID, int fromID, string toIDs, others vars...

static class DataSender
{
    public static void SendData(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
        buffer.WriteBytes(data);
        Client.stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
        buffer.Dispose();
    }

    #region Packet methods
    public static void SendEcho (string to)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)Packets.CEcho);
        buffer.WriteInteger(Client.connectionID);
        buffer.WriteString(to);
        buffer.WriteString(DateTime.Now.ToString());
        SendData(buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendInstantiate (Vector3 pos, string to, bool calledForResponse = false)
    {
        // methodID, fromID, to, spawnPos
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)Packets.CInstantiate);
        buffer.WriteInteger(Client.connectionID);
        buffer.WriteString(to);
        buffer.WriteString($"{pos.x};{pos.y};{pos.z}");
        buffer.WriteBool(calledForResponse);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendUpdateTransform (Vector3 position, float zRot)
    {
        // methodID, fromID, to, pos and z rot (for 2D games)
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)Packets.CUpdateTransform);
        buffer.WriteInteger(Client.connectionID);
        buffer.WriteString("others");
        buffer.WriteString($"{position.x};{position.y};{position.z};{zRot}");
        SendData(buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendTakeDamages (int targetID, float damages)
    {
        // methodID, fromID, to, damages
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)Packets.CTakeDamages);
        buffer.WriteInteger(Client.connectionID);
        buffer.WriteString($"to {targetID}");
        buffer.WriteFloat(damages);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }

    public static void SendDie (int killerID)
    {
        // methodID, fromID, to, killerID
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteInteger((int)Packets.CDie);
        buffer.WriteInteger(Client.connectionID);
        buffer.WriteString("others");
        buffer.WriteInteger(killerID);
        SendData(buffer.ToArray());
        buffer.Dispose();
    }
    #endregion
}
