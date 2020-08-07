using System;
using System.Net.Sockets;
using UnityEngine;

static class Client
{
    //CLIENT PROPERTIES
    public static string ip = "127.0.0.1";
    public static int port = 4456;

    public static int connectionID;
    public static TcpClient socket;
    public static NetworkStream stream;
    public static byte[] receiveBuffer;

    //INITIALIZE METHOD
    public static void Initialize (string _ip, int _port)
    {
        ip = _ip;
        port = _port;

        socket = new TcpClient();
        socket.ReceiveBufferSize = 4096;
        socket.SendBufferSize = 4096;
        receiveBuffer = new byte[2 * 4096];

        var result = socket.BeginConnect(ip, port, new AsyncCallback(OnConnected), socket);

        var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
        if (!success)
        {
            UIManager.instance.ShowError($"Impossible de se connecter au server suivant : {ip}:{port}");
            Debug.Log($"Can't find a server at given adress ({ip}:{port}), please verify your connection and the server status.");
        }
    }


    //CONNECT CALLBACK and DISCONNECT METHOD
    private static void OnConnected (IAsyncResult result)
    {
        socket.EndConnect(result);
        if (socket.Connected == false)
        {
            UIManager.instance.ShowError($"Impossible de se connecter au server suivant : {ip}:{port}");
            Debug.Log($"Can't find a server at given adress ({ip}:{port}), please verify your connection and the server status.");
            return;
        }
        else
        {
            socket.NoDelay = true;
            stream = socket.GetStream();
            stream.BeginRead(receiveBuffer, 0, 4096 * 2, OnReceiveData, null);
            Debug.Log($"Connected to server {ip}:{port}");
        }
    }

    public static void Disconnect()
    {
        if (socket.Connected)
        {
            UIManager.instance.ShowError("Vous étes maintenant deconnecté du server.");
            Debug.Log("Deconnected from server.");
        }
        socket.Close();
    }


    //RECEIVE DATA CALLBACK
    private static void OnReceiveData(IAsyncResult result)
    {
        try
        {
            int length = stream.EndRead(result);
            if (length <= 0)
            {
                return;
            }

            byte[] newBytes = new byte[length];
            Array.Copy(receiveBuffer, newBytes, length);
            UnityThread.executeInFixedUpdate(() =>
            {
                DataReceiver.HandleData(newBytes);
            });

            stream.BeginRead(receiveBuffer, 0, 4096 * 2, OnReceiveData, null);
            DataReceiver.playerBuffer.Clear();
        }
        catch (Exception)
        {
            return;
        }
    }
}
