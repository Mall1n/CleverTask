using System.Text;
using System.Text.RegularExpressions;

namespace Task1
{
    public static class CompressionEx
    {
        public static string CompressByRegex(string text)
        {
            const string pattern = @"(.)\1+";
            return Regex.Replace(text, pattern, match =>
                $"{match.Groups[1].Value}{match.Length}");
        }

        public static string Compress(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            StringBuilder result = new();
            int count = 1;

            for (int i = 0; i < text.Length; i++)
            {
                if (i + 1 < text.Length && text[i] == text[i + 1])
                {
                    count++;
                }
                else
                {
                    result.Append(text[i]);
                    if (count > 1)
                        result.Append(count);
                    count = 1;
                }
            }

            return result.ToString();
        }

        public static string Decompress(string text)
        {
            StringBuilder result = new();
            int i = 0;

            while (i < text.Length)
            {
                char currentChar = text[i];
                i++;

                StringBuilder numberBuilder = new();
                while (i < text.Length && char.IsDigit(text[i]))
                {
                    numberBuilder.Append(text[i]);
                    i++;
                }

                if (numberBuilder.Length > 0)
                {
                    int count = int.Parse(numberBuilder.ToString());
                    result.Append(new string(currentChar, count));
                }
                else
                {
                    result.Append(currentChar);
                }
            }

            return result.ToString();
        }
    }
}
