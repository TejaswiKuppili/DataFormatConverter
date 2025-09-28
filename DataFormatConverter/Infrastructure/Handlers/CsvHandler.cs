using CsvHelper;
using DataFormatConverter.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFormatConverter.Infrastructure.Handlers
{
    /// <summary>
    /// Handles CSV serialization/deserialization for lists of dictionaries.
    /// </summary>
    public class CsvHandler : IFormatHandler
    {
        public string FormatKey => "csv";

        public object Deserialize(string data)
        {
            using var reader = new StringReader(data);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = new List<Dictionary<string, object>>();

            csv.Read();
            csv.ReadHeader();
            var headers = csv.HeaderRecord;

            while (csv.Read())
            {
                var record = new Dictionary<string, object>();
                foreach (var header in headers)
                    record[header] = csv.GetField(header);
                records.Add(record);
            }

            return records;
        }

        public string Serialize(object obj)
        {
            using var writer = new StringWriter();
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            IEnumerable<Dictionary<string, object>> listToWrite = null;

            if (obj is IEnumerable<Dictionary<string, object>> dictList)
            {
                listToWrite = dictList;
            }
            else if (obj is IEnumerable<object> objList)
            {
                listToWrite = objList
                    .OfType<IDictionary<string, object>>()
                    .Select(d => d.ToDictionary(k => k.Key, v => v.Value))
                    .ToList();
            }
            else if (obj is IDictionary<string, object> singleDict)
            {
                listToWrite = new List<Dictionary<string, object>> { singleDict.ToDictionary(k => k.Key, v => v.Value) };
            }
            else
            {
                // Single primitive
                csv.WriteField(obj?.ToString());
                csv.NextRecord();
                return writer.ToString();
            }

            if (listToWrite != null && listToWrite.Any())
            {
                // Write headers
                foreach (var kv in listToWrite.First())
                    csv.WriteField(kv.Key);
                csv.NextRecord();

                // Write rows
                foreach (var record in listToWrite)
                {
                    foreach (var kv in record)
                        csv.WriteField(kv.Value);
                    csv.NextRecord();
                }
            }

            return writer.ToString();
        }
    }
}
