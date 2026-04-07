using System.Text.RegularExpressions;

namespace Task3
{
    public static class Parser
    {
        private static readonly Regex regex = new(@"^(?<date>\d{2,4}[.-]\d{2}[.-]\d{2,4})[\t |]+(?<time>\d{2}:\d{2}:\d{2}\.\d+)[\t |]+(?<level>[A-Z]+)(?:[\t |]+(?<method>[^\s]+))?[\t |]+(?<message>[^\s]+ +[^\s]+: +'.+')", RegexOptions.Compiled);

        public static bool TryParse(string text, out LogEntry logEntry)
        {
            var match = regex.Match(text);
            logEntry = null;

            if (match.Success)
            {
                if (DateTime.TryParse(match.Groups["date"].Value, null, out DateTime dateTime))
                {
                    //Console.WriteLine("method: " + (match.Groups["method"].Success ? match.Groups["method"].Value : "DEFAULT"));
                    logEntry = new LogEntry()
                    {
                        date = dateTime,
                        time = match.Groups["time"].Value,
                        level = match.Groups["level"].Value,
                        method = match.Groups["method"].Success ? match.Groups["method"].Value : "DEFAULT",
                        message = match.Groups["message"].Value
                    };

                    if (!logEntry.IsValidLog())
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }

    public class LogEntry
    {
        public DateTime date;
        public string time;
        public string level;
        public string method;
        public string message;

        public bool IsValidLog()
        {
            this.level = NormalizeLevel(level);
            if (this.level == UNKNOWN_LEVEL)
                return false;

            return true;
        }

        public override string ToString()
        {
            return $"{date.ToString("yyyy-MM-dd")} {time} {level} {method} {message}";
        }

        private const string UNKNOWN_LEVEL = "UNKNOWN";

        private static string NormalizeLevel(string level)
        {
            return level.ToUpper() switch
            {
                "INFORMATION" => "INFO",
                "INFO" => "INFO",
                "WARNING" => "WARN",
                "WARN" => "WARN",
                "ERROR" => "ERROR",
                "DEBUG" => "DEBUG",
                _ => UNKNOWN_LEVEL
            };
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            ParseLogInternal("2025-03-10\t15:14:49.523\tINFO\tDEFAULT\tВерсия программы: \'3.4.0.48729\'");
            ParseLogInternal("10.03.2025 15:14:49.523 INFORMATION Версия программы: \'3.4.0.48729\'");
            ParseLogInternal("2025-03-10 15:14:51.5882|WARNING|MobileComputer.GetDeviceId|Код устройства: '@MINDEO-M40-D-410244015546'");
            ParseLogInternal("2025-03-10 15:14:51.5882|INFO123|MobileComputer.GetDeviceId|Код устройства: '@MINDEO-M40-D-410244015546'");

            while (true) 
            {
                string input = Console.ReadLine() ?? string.Empty;

                ParseLogInternal(input);
            }
        }

        private static void ParseLogInternal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            if (Parser.TryParse(input, out LogEntry logOut))
            {
                string result = logOut.ToString() ?? string.Empty;
                Console.WriteLine(result);
                File.AppendAllText("output.txt", result + "\n");
            }
            else
            {
                Console.WriteLine("Error while parsing log");
                File.AppendAllText("problems.txt", input + "\n");
            }

            Console.WriteLine();
        }
    }
}
