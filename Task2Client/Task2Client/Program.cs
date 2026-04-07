using System.Net.Sockets;
using System.Text;

namespace Task2Client
{
    internal class Program
    {
        private readonly static int _port = 8888;

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== TCP Клиент ===");

            try
            {
                await ConnectToServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        private static async Task ConnectToServer()
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    Console.WriteLine("Подключение к серверу...");
                    client.Connect("127.0.0.1", _port);

                    Console.WriteLine("Подключение установлено!");
                    Console.WriteLine("Введите сообщения для отправки серверу:\n");

                    using (NetworkStream stream = client.GetStream())
                    {
                        var receiveTask = ReceiveMessages(stream);
                        await SendMessages(stream);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static async Task SendMessages(NetworkStream stream)
        {
            try
            {
                using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                while (true)
                {
                    string message = Console.ReadLine() ?? string.Empty;

                    if (string.IsNullOrEmpty(message))
                        continue;

                    await writer.WriteLineAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке: {ex.Message}");
            }
        }

        private static async Task ReceiveMessages(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        Console.WriteLine("\nСервер отключился");
                        break;
                    }

                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Ответ сервера: {response.Trim()}");
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
