using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Task2
{
    public class Server : IDisposable
    {
        private TcpListener _listener;
        private bool _isRunning = false;
        private CancellationTokenSource _cancellationTokenSource;

        private const int PORT = 8888;

        private int _count = 0;

        public void Start()
        {
            StartInternal();
        }

        private void StartInternal()
        {
            if (_isRunning)
                return;

            CancelToken();

            _cancellationTokenSource = new();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            Task.Run(() => ServerThread(cancellationToken));
        }

        public void Stop()
        {
            StopInternal();
        }

        public void Dispose()
        {
            StopInternal();
        }

        private void StopInternal()
        {
            if (!_isRunning)
                return;

            _count = 0;
            CancelToken();
            _isRunning = false;
            _listener?.Stop();

            Console.WriteLine("Server stopped");
        }

        private void CancelToken()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private async Task ServerThread(CancellationToken cancellationToken = default)
        {
            _isRunning = true;

            _listener = new TcpListener(IPAddress.Any, PORT);
            _listener.Start();

            Console.WriteLine($"Сервер запущен на порту {PORT}");
            Console.WriteLine($"Ожидание подключений...\n");

            while (_isRunning)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await HandleClient(client);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Client error: {ex}");
                        }
                    });
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при принятии подключения: {ex.Message}");
                }
            }

            Stop();
        }

        private async Task HandleClient(TcpClient client)
        {
            string clientId = $"{((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}";

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Клиент подключен: {clientId}");

            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                try
                {
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                    while (client.Connected)
                    {
                        string message = await reader.ReadLineAsync() ?? string.Empty;
                        Console.WriteLine($"Получено [{clientId}]: {message}");

                        string echo = string.Empty;

                        if (message.StartsWith("get"))
                        {
                            int count = GetCount();
                            Console.WriteLine($"Сервер вернул значение {count} клиенту {clientId}");
                            echo = $"Получено значение с сервера: {count}";
                        }
                        else if (message.StartsWith("add "))
                        {
                            string[] strs = message.Split(' ');
                            if (strs.Length >= 2)
                            {
                                if (int.TryParse(strs[1], out int result))
                                {
                                    AddToCount(result);
                                    Console.WriteLine($"Сервер увеличил значение на {result}. Значение = {_count}. Клиент {clientId}");
                                    echo = $"Echo: added: {result}. Count: {_count}";
                                }
                            }
                        }
                        else
                        {
                            echo = "сообщение доставлено";
                        }

                        await writer.WriteLineAsync(echo);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Ошибка с клиентом {clientId}: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Клиент отключен: {clientId}\n");
                }
            }
        }

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public void AddToCount(int value)
        {
            _lock.EnterWriteLock();
            try
            {
                Thread.Sleep(5000); // имитация работы

                _count += value;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public int GetCount()
        {
            _lock.EnterReadLock();
            try
            {
                //Thread.Sleep(3000); // имитация работы

                return _count;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
