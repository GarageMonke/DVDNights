namespace Code.Common.Localization
{
    using System.Collections.Generic;
    using System.Text;

    public static class CsvParser
    {
        public static Dictionary<string, Dictionary<string, string>> Parse(string csvText)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            var lines = SplitLines(csvText);
            if (lines.Count == 0) return result;

            var headers = ParseLine(lines[0]);
            for (int col = 1; col < headers.Count; col++)
                result[headers[col].Trim()] = new Dictionary<string, string>();

            for (int row = 1; row < lines.Count; row++)
            {
                if (string.IsNullOrWhiteSpace(lines[row])) continue;
                var fields = ParseLine(lines[row]);
                if (fields.Count == 0) continue;

                string key = fields[0].Trim();
                if (string.IsNullOrEmpty(key)) continue;

                for (int col = 1; col < headers.Count && col < fields.Count; col++)
                {
                    string lang = headers[col].Trim();
                    result[lang][key] = fields[col];
                }
            }

            return result;
        }

        private static List<string> SplitLines(string text)
        {
            // Split on \n but respect quoted fields that may contain literal newlines
            var lines = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            foreach (char c in text)
            {
                if (c == '"') inQuotes = !inQuotes;

                if (c == '\n' && !inQuotes)
                {
                    lines.Add(current.ToString());
                    current.Clear();
                }
                else if (c != '\r')
                {
                    current.Append(c);
                }
            }

            if (current.Length > 0) lines.Add(current.ToString());
            return lines;
        }

        private static List<string> ParseLine(string line)
        {
            var result = new List<string>();
            var field = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        field.Append('"'); // escaped quote
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }

                    continue;
                }

                if (c == ',' && !inQuotes)
                {
                    result.Add(field.ToString());
                    field.Clear();
                    continue;
                }

                field.Append(c);
            }

            result.Add(field.ToString());
            return result;
        }
    }
}