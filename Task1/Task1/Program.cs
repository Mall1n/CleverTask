using System.Text;
using System.Text.RegularExpressions;

namespace Task1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter string:");
                string input = Console.ReadLine() ?? string.Empty;

                if (!IsInputValid(input))
                    continue;

                Console.WriteLine();
                Console.WriteLine($"{"Input = ",-30}{input}");
                Console.WriteLine();

                string compressed = Compress(input);

                Decompress(compressed);

                Console.WriteLine(new string('-', 50));
                Console.WriteLine();
            }

        }

        private static void Decompress(string input)
        {
            ConsoleWriteLineColor($"Decompressing...");
            Console.WriteLine();

            string decompressed = CompressionEx.Decompress(input);

            Console.WriteLine($"{"Decompressed input = ",-30}{decompressed}");
            Console.WriteLine();
        }

        private static string Compress(string input)
        {
            ConsoleWriteLineColor($"Compressing...");
            Console.WriteLine();

            string compressedRegex = CompressionEx.CompressByRegex(input);
            string compressed = CompressionEx.Compress(input);

            Console.WriteLine($"{"Compressed input by regex = ",-30}{compressedRegex}");
            Console.WriteLine($"{"Compressed input = ",-30}{compressed}");
            Console.WriteLine();

            return compressed;
        }

        private static bool IsInputValid(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                ConsoleWriteLineColor("Input has not allowed symbols! Try again...", ConsoleColor.Yellow);
                return false;
            }

            if (IsHasAnyDigitals(text))
            {
                ConsoleWriteLineColor("Input has any digitals. It's not allowed! Try again...", ConsoleColor.Yellow);
                Console.WriteLine();
                return false;
            }

            if (IsHasUpperCase(text))
            {
                ConsoleWriteLineColor("Input has any upper case. It's not allowed! Try again...", ConsoleColor.Yellow);
                Console.WriteLine();
                return false;
            }

            return true;
        }

        private static bool IsHasAnyDigitals(string text)
        {
            bool hasDigits = Regex.IsMatch(text, @"[0-9]");
            return hasDigits;
        }

        private static bool IsHasUpperCase(string text)
        {
            bool hasUpperCase = text.Any(char.IsUpper);
            return hasUpperCase;
        }

        private static void ConsoleWriteLineColor(string log, ConsoleColor consoleColor = ConsoleColor.Green)
        {
            ConsoleColor consoleColorPrev = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;

            Console.WriteLine(log);

            Console.ForegroundColor = consoleColorPrev;
        }
    }
}
