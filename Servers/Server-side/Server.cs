using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
 
namespace Server
{
    static class Server
    {
        public const string ip = "127.0.0.1";
        public const int port = 4456;
        private static TcpListener socket = new TcpListener(IPAddress.Any, port);

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public static void Start ()
        {
            Console.WriteLine("Starting server, please wait...");
            DataReceiver.InitializePacketsMethods();
            socket.Start();
            socket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
            Console.WriteLine("Ready to receive packets");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Server has started on {ip}:{port}");
            Console.ForegroundColor = ConsoleColor.White;
        }


        private static void OnClientConnect(IAsyncResult result)
        {
            TcpClient client = socket.EndAcceptTcpClient(result);
            socket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);

            Client newClient = new Client();
            newClient.socket = client;
            newClient.connexionID = ((IPEndPoint)client.Client.RemoteEndPoint).Port;
            newClient.Initialize();
            clients.Add(newClient.connexionID, newClient);

            DataSender.SendWelcomeMessage(newClient.connexionID);
        }


        public static int[] GetClientsID (int[] except = null)
        {
            List<int> ids = new List<int>();
            foreach (Client client in clients.Values)
            {
                if(except != null)
                {
                    bool isInExcept = false;
                    foreach (int exceptID in except)
                    {
                        if(exceptID == client.connexionID)
                        {
                            isInExcept = true;
                            break;
                        }
                    }

                    if (!isInExcept)
                    {
                        ids.Add(client.connexionID);
                    }
                }
                else
                {
                    ids.Add(client.connexionID);
                }
            }
            return ids.ToArray();
        }
    }
}
