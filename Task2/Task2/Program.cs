using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Task2
{
    internal class Program
    {
        private static Server server;

        static void Main(string[] args)
        {
            Console.WriteLine("=== Сервер ===");
            Console.WriteLine("Type \"start\" for start server...\n");

            server = new Server();

            while (true)
            {
                string? input = Console.ReadLine();

                if (input == null || string.IsNullOrEmpty(input))
                    continue;

                switch (input)
                {
                    case "start":
                        server.Start();
                        break;

                    case "stop":
                        server.Stop();
                        break;

                    case "exit":
                        server.Stop();
                        Environment.Exit(0);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
