using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataFormatConverter.Helper
{
    public static class DataNormalizer
    {
        /// <summary>
        /// Converts the 'data' object from the request into a string
        /// so that it can be passed to the format handlers.
        /// </summary>
        public static string ExtractDataAsString(object data)
        {
            switch (data)
            {
                case null:
                    return string.Empty;

                // Case 1: Direct string (e.g. XML, CSV, or already serialized JSON)
                case string str:
                    return str;

                // Case 2: JSON object or array automatically bound by ASP.NET Core
                case JsonElement json:
                    return json.GetRawText();

                // Case 3: Already deserialized object (edge case)
                default:
                    return JsonSerializer.Serialize(data);
            }
        }
    }
}
