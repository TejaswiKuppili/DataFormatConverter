using DataFormatConverter.Domain.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataFormatConverter.Infrastructure.Handlers
{
    /// <summary>
    /// Handles XML serialization and deserialization.
    /// </summary>
    public class XmlHandler : IFormatHandler
    {
        public string FormatKey => "xml";

        public object Deserialize(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return new Dictionary<string, object>();

            data = data.Trim();

            // Safely unescape JSON string if it has surrounding quotes
            if (data.StartsWith("\"") && data.EndsWith("\""))
            {
                data = JsonSerializer.Deserialize<string>(data);
            }

            var doc = XDocument.Parse(data);
            var root = doc.Root;

            if (root == null)
                return new Dictionary<string, object>();

            // Recursive conversion
            return new Dictionary<string, object>
            {
                [root.Name.LocalName] = ParseElement(root)
            };
        }

        private object ParseElement(XElement element)
        {
            // If element has no children → return text value
            if (!element.HasElements)
                return element.Value;

            // If element has multiple children with the same name → treat as array
            var grouped = element.Elements().GroupBy(e => e.Name.LocalName);
            bool isArray = grouped.Any(g => g.Count() > 1);

            if (isArray)
            {
                var dict = new Dictionary<string, object>();
                foreach (var g in grouped)
                {
                    if (g.Count() > 1)
                    {
                        dict[g.Key] = g.Select(ParseElement).ToList();
                    }
                    else
                    {
                        dict[g.Key] = ParseElement(g.First());
                    }
                }
                return dict;
            }
            else
            {
                // Otherwise, treat as single nested object
                var dict = new Dictionary<string, object>();
                foreach (var child in element.Elements())
                    dict[child.Name.LocalName] = ParseElement(child);
                return dict;
            }
        }

        //public object Deserialize(string data)
        //{
        //    if (string.IsNullOrWhiteSpace(data))
        //        return new Dictionary<string, object>();

        //    // Remove surrounding quotes if data came from JSON string
        //    data = data.Trim();
        //    if (data.StartsWith("\"") && data.EndsWith("\""))
        //    {
        //        data = data.Substring(1, data.Length - 2)
        //                   .Replace("\\n", "")
        //                   .Replace("\\t", "");
        //    }

        //    var doc = XDocument.Parse(data);
        //    var root = doc.Root;

        //    if (root == null)
        //        return new Dictionary<string, object>();

        //    // If root has multiple child elements with the same name, treat as list
        //    if (root.Elements().GroupBy(e => e.Name.LocalName).Any(g => g.Count() > 1))
        //    {
        //        var list = new List<Dictionary<string, object>>();
        //        foreach (var item in root.Elements())
        //        {
        //            var dict = item.Elements().ToDictionary(x => x.Name.LocalName, x => (object)x.Value);
        //            list.Add(dict);
        //        }
        //        return list;
        //    }

        //    // Otherwise, single object
        //    return root.Elements().ToDictionary(x => x.Name.LocalName, x => (object)x.Value);
        //}

        public string Serialize(object obj)
        {
            var root = new XElement("Root");
            AddElements(root, obj);
            return new XDocument(root).ToString();
        }

        // Recursive helper method
        private void AddElements(XElement parent, object obj)
        {
            if (obj is IDictionary<string, object> dict)
            {
                foreach (var kv in dict)
                {
                    if (kv.Value is IDictionary<string, object> || kv.Value is IEnumerable<object>)
                    {
                        var child = new XElement(kv.Key);
                        AddElements(child, kv.Value);
                        parent.Add(child);
                    }
                    else
                    {
                        parent.Add(new XElement(kv.Key, kv.Value?.ToString()));
                    }
                }
            }
            else if (obj is IEnumerable<object> list)
            {
                foreach (var item in list)
                {
                    var child = new XElement("Item");
                    AddElements(child, item);
                    parent.Add(child);
                }
            }
            else
            {
                parent.Value = obj?.ToString();
            }
        }

    }
}
