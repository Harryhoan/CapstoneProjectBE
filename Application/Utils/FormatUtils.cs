using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Application.Utils
{
    public class FormatUtils
    {
        public static string TrimSpacesPreserveSingle(string input)
        {
            return Regex.Replace(input.Trim(), @"\s+", " ");
        }

        public static string CapitalizeWords(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string[] words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder result = new();

            foreach (string word in words)
            {
                string capitalizedWord = textInfo.ToTitleCase(word.ToLower());
                result.Append(capitalizedWord).Append(' ');
            }

            return result.ToString().Trim();
        }

        public static string FormatText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input ?? string.Empty;
            }

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

            // Normalize whitespace (preserve newlines)
            string cleanedInput = Regex.Replace(input, @"[^\S\n]+", " ").Trim();

            // Handle punctuation spacing (ensures space after punctuation, but not before)
            cleanedInput = Regex.Replace(cleanedInput, @"([,.!?:;])([^\s"",.!?:;])", "$1 $2");
            cleanedInput = Regex.Replace(cleanedInput, @"([,.!?:;])\s+([,.!?:;])", "$1$2");

            // Normalize quotes (trim internal spaces, ensure proper pairing)
            cleanedInput = Regex.Replace(cleanedInput, @"\s*([""'])\s*", "$1");
            cleanedInput = Regex.Replace(cleanedInput, @"([""'])([^\s])", "$1 $2");
            cleanedInput = Regex.Replace(cleanedInput, @"([^\s])([""'])", "$1 $2");

            // Split into paragraphs
            string[] paragraphs = cleanedInput.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder result = new StringBuilder();

            foreach (string paragraph in paragraphs)
            {
                string trimmedParagraph = paragraph.Trim();
                if (string.IsNullOrWhiteSpace(trimmedParagraph))
                {
                    continue;
                }

                // Ensure paragraph ends with sentence terminator
                if (!Regex.IsMatch(trimmedParagraph, @"[.!?]\s*$"))
                {
                    trimmedParagraph += ".";
                }

                // Split sentences more accurately (handles abbreviations like "Dr.")
                string[] sentences = Regex.Split(trimmedParagraph, @"(?<!\w\.\w.)(?<![A-Z][a-z]\.)(?<=[.!?])\s+");
                StringBuilder paragraphBuilder = new StringBuilder();

                foreach (string sentence in sentences)
                {
                    string trimmedSentence = sentence.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedSentence))
                    {
                        continue;
                    }

                    // Capitalize first letter while preserving proper nouns
                    if (trimmedSentence.Length > 0)
                    {
                        trimmedSentence = char.ToUpper(trimmedSentence[0]) +
                                        (trimmedSentence.Length > 1 ? trimmedSentence.Substring(1) : "");
                    }

                    // Ensure proper spacing after commas
                    trimmedSentence = Regex.Replace(trimmedSentence, @",([^\s])", ", $1");

                    paragraphBuilder.Append(trimmedSentence);

                    // Add space unless ending with punctuation
                    if (!Regex.IsMatch(trimmedSentence, @"[.!?]$"))
                    {
                        paragraphBuilder.Append(' ');
                    }
                }

                // Finalize paragraph with proper capitalization
                string formattedParagraph = paragraphBuilder.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(formattedParagraph))
                {
                    result.AppendLine(formattedParagraph);
                }
            }

            // Normalize line breaks and final cleanup
            string finalResult = Regex.Replace(result.ToString(), @"\n{3,}", "\n\n").Trim();

            // Final pass for any remaining spacing issues
            finalResult = Regex.Replace(finalResult, @"\s+([,.!?:;])", "$1");
            finalResult = Regex.Replace(finalResult, @"([,.!?:;])([^\s])", "$1 $2");
            finalResult = Regex.Replace(finalResult, @"(['""])(.*?)\1", m =>
            {
                // Trim spaces inside the matched quote (m.Groups[2] contains the quoted content)
                return m.Groups[1].Value + m.Groups[2].Value.Trim() + m.Groups[1].Value;
            });

            return finalResult;
        }
        public static string SafeUrlDecode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Check if the string contains any valid URL-encoded sequences
            bool isEncoded = false;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '%' && i + 2 < input.Length)
                {
                    if (IsHexDigit(input[i + 1]) && IsHexDigit(input[i + 2]))
                    {
                        isEncoded = true;
                        break;
                    }
                }
                else if (input[i] == '+' && !isEncoded)
                {
                    // Only consider + as encoding if we find no % sequences
                    isEncoded = true;
                }
            }

            if (!isEncoded)
                return input;

            try
            {
                string decoded = HttpUtility.UrlDecode(input);

                // Verify decoding didn't create new % sequences (which would indicate double encoding)
                if (decoded.Contains("%") && decoded != input)
                {
                    bool hasNewEncoding = false;
                    for (int i = 0; i < decoded.Length; i++)
                    {
                        if (decoded[i] == '%' && i + 2 < decoded.Length &&
                            IsHexDigit(decoded[i + 1]) && IsHexDigit(decoded[i + 2]))
                        {
                            hasNewEncoding = true;
                            break;
                        }
                    }

                    if (hasNewEncoding)
                        return input; // Probably double-encoded, return original
                }

                return decoded;
            }
            catch
            {
                return input; // If decoding fails, return original
            }
        }

        private static bool IsHexDigit(char c)
        {
            return (c >= '0' && c <= '9') ||
                   (c >= 'a' && c <= 'f') ||
                   (c >= 'A' && c <= 'F');
        }
    }
}
