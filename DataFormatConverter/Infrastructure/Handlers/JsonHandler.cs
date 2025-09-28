using DataFormatConverter.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataFormatConverter.Infrastructure.Handlers
{
    /// <summary>
    /// Handles JSON serialization/deserialization for any object.
    /// Converts JSON into Dictionary or List of Dictionary for compatibility.
    /// </summary>
    public class JsonHandler : IFormatHandler
    {
        public string FormatKey => "json";

        public object Deserialize(string data)
        {
            var jsonDoc = JsonDocument.Parse(data);
            return ConvertJsonElement(jsonDoc.RootElement);
        }

        public string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        private object ConvertJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var dict = new Dictionary<string, object>();
                    foreach (var prop in element.EnumerateObject())
                        dict[prop.Name] = ConvertJsonElement(prop.Value);
                    return dict;

                case JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var item in element.EnumerateArray())
                        list.Add(ConvertJsonElement(item));
                    return list;

                case JsonValueKind.String:
                    return element.GetString();

                case JsonValueKind.Number:
                    return element.GetDouble();

                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();

                case JsonValueKind.Null:
                    return null;

                default:
                    return element.ToString();
            }
        }
    }
}
