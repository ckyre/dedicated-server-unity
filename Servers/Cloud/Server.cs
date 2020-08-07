using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    static class Server
    {
        //SERVER PROPERTIES
        public static bool isRunning = false;
        public const int port = 4456;
        public static TcpListener socket = new TcpListener(IPAddress.Any, port);

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();


        //START and STOP METHODS
        public static void Start ()
        {
            if (!isRunning)
            {
                socket.Start();
                socket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
                isRunning = true;
                Debug.ImportantMessage($"Server has started on {port}");
            }
        }

        public static void Stop ()
        {
            if (isRunning)
            {
                socket.Stop();
                isRunning = false;
                Debug.ImportantMessage("Server has been stopped. Press any key to close this window.");
            }
        }


        //CONNECTION and DECONNECTION CALLBACKS
        public static void OnClientConnect (IAsyncResult result)
        {
            if (isRunning)
            {
                TcpClient clientSocket = socket.EndAcceptTcpClient(result);
                int clientID = ((IPEndPoint)clientSocket.Client.RemoteEndPoint).Port;
                Client client = new Client(clientSocket, clientID, 4096);
                clients.Add(clientID, client);
                Debug.Message($"Incoming connection from {((IPEndPoint)client.socket.Client.RemoteEndPoint).Port}");

                //Send initialization packet to new client
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteInteger(1);
                buffer.WriteInteger(clientID);
                SendData(new int[] {clientID}, buffer.ToArray());
                buffer.Dispose();

                socket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
            }
        }

        public static void OnClientDisconnect (Client client)
        {
            if (isRunning && clients.Keys.ToArray().Contains(client.id))
            {
                Debug.Message($"Client {((IPEndPoint)client.socket.Client.RemoteEndPoint).Port} is now deconnected from the server.");

                //Send to others clients
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteInteger(6);
                buffer.WriteInteger(client.id);
                buffer.WriteString("others");
                buffer.WriteInteger(0);
                SendData(StringToIDList(client.id, "others"), buffer.ToArray());
                buffer.Dispose();

                //Close connection
                client.socket.Close();
                clients.Remove(client.id);
            }
        }


        //RECEIVE CALLBACK and SEND METHOD
        public static void OnReceiveData (byte[] data)
        {
            if (isRunning)
            {
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteBytes(data);
                int packetLength = buffer.ReadInteger();
                int methodID = buffer.ReadInteger();
                int fromID = buffer.ReadInteger();
                string to = buffer.ReadString();
                buffer.Dispose();

                SendData(StringToIDList(fromID, to), data, false);
            }
        }

        public static void SendData (int[] targets, byte[] data, bool addLength = true)
        {
            if (isRunning)
            {
                foreach (int target in targets)
                {
                    if (clients[target] != null)
                    {
                        if (addLength)
                        {
                            ByteBuffer buffer = new ByteBuffer();
                            buffer.WriteInteger((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
                            buffer.WriteBytes(data);
                            clients[target].stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
                            buffer.Dispose();
                        }else
                        {
                            clients[target].stream.BeginWrite(data, 0, data.Length, null, null);
                        }
                    }
                }
            }
        }


        //OTHERS METHODS
        public static int[] StringToIDList(int fromID, string input)
        {
            if(input == "me")
            {
                return new int[] { fromID };

            }else if (input == "all")
            {
                return clients.Keys.ToArray();
            }else if (input == "others")
            {
                List<int> keys = clients.Keys.ToList();
                keys.RemoveAt(keys.IndexOf(fromID));
                return keys.ToArray();
            }else if (input.Contains("to"))
            {
                int toID = int.Parse(input.Replace("to ", ""));
                return new int[] { toID };
            }else
            {
                return new int[0];
            }
        }

    }
}
