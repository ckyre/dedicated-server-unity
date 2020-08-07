using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";
            Server.Start();
            Console.ReadKey();
        }
    }
}
