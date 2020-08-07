using System;
using System.Linq;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";
            Debug.Message("Starting server, please wait...");
            Server.Start();

            WaitForCommands();
        }

        private static void WaitForCommands()
        {
            string command = Console.ReadLine();
            command = command.ToLower();

            if (command == "stop")
            {
                Server.Stop();
                Console.ReadKey();
            }
            else if (command == "help")
            {
                Debug.Message("Commands list :");
                Debug.Message("  - help : show this list");
                Debug.Message("  - stop : close the server");
                Debug.Message("  - count : show the players count");
                Debug.Message("  - kick <player name> : kick a player from the server");
                WaitForCommands();
            }
            else if (command == "count")
            {
                Debug.Message($"Current players on server : {Server.clients.Count}");
                WaitForCommands();
            }
            else if (command.Contains("kick"))
            {
                string playerName = command.Replace("kick", "");
                if(playerName != null & playerName != "" & playerName != " ")
                {
                    int playerID = int.Parse(playerName);
                    if (Server.clients.Keys.ToArray().Contains(playerID))
                    {
                        Server.OnClientDisconnect(Server.clients[playerID]);
                    }
                    else { Debug.ErrorMessage($"Unable to find client {playerID}"); }
                }else { Debug.ErrorMessage("Please fill in a customer name to kick a player."); }
                WaitForCommands();
            }
            else if (command == "hello")
            {
                Debug.Message("Hello!");
                WaitForCommands();
            }
            else
            {
                Debug.ErrorMessage("Unknown command! type help to get the commands list.");
                WaitForCommands();
            }
        }
    }
}
