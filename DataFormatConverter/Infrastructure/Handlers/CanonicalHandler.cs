using DataFormatConverter.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataFormatConverter.Infrastructure.Handlers
{
    /// <summary>
    /// Handles serialization and deserialization for Canonical String format.
    /// Canonical String Format: field1=value1|field2=value2|field3=value3
    /// or a simple concatenation with '|'.
    /// </summary>
    public class CanonicalHandler : IFormatHandler
    {
        public string FormatKey => "canonical";

        /// <summary>
        /// Converts a canonical string into a dictionary representation.
        /// Expected format: key1=value1|key2=value2|key3=value3
        /// </summary>
        public object Deserialize(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException("Canonical data cannot be null or empty.");

            var records = new List<Dictionary<string, object>>();

            // Handle multi-line canonical data (optional)
            var lines = data.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var record = new Dictionary<string, object>();
                var pairs = line.Split('|', StringSplitOptions.RemoveEmptyEntries);

                foreach (var pair in pairs)
                {
                    // If format is key=value
                    if (pair.Contains('='))
                    {
                        var kv = pair.Split('=', 2);
                        record[kv[0].Trim()] = kv[1].Trim();
                    }
                    else
                    {
                        // If only values (no keys)
                        record[$"Field{record.Count + 1}"] = pair.Trim();
                    }
                }

                records.Add(record);
            }

            return records.Count == 1 ? records[0] : records;
        }

        /// <summary>
        /// Converts a dictionary or list of dictionaries into a canonical string.
        /// Output format: key1=value1|key2=value2|key3=value3
        /// </summary>
        public string Serialize(object obj)
        {
            if (obj is IEnumerable<Dictionary<string, object>> dictList)
            {
                return string.Join(Environment.NewLine, dictList.Select(ConvertToCanonicalLine));
            }
            else if (obj is IDictionary<string, object> singleDict)
            {
                return ConvertToCanonicalLine(singleDict);
            }
            else
            {
                // Handle primitive or unknown types
                return obj?.ToString() ?? string.Empty;
            }
        }

        private static string ConvertToCanonicalLine(IDictionary<string, object> dict)
        {
            return string.Join("|", dict.Select(kv => $"{kv.Key}={kv.Value}"));
        }
    }
}
