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

                case string str:
                    return str;

                case JsonElement json:
                    return json.GetRawText();

                default:
                    return JsonSerializer.Serialize(data);
            }
        }
    }
}
