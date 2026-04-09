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

            if (data.StartsWith("\"") && data.EndsWith("\""))
            {
                data = JsonSerializer.Deserialize<string>(data);
            }

            var doc = XDocument.Parse(data);
            var root = doc.Root;

            if (root == null)
                return new Dictionary<string, object>();

            return new Dictionary<string, object>
            {
                [root.Name.LocalName] = ParseElement(root)
            };
        }

        private object ParseElement(XElement element)
        {
            if (!element.HasElements)
                return element.Value;

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
                var dict = new Dictionary<string, object>();
                foreach (var child in element.Elements())
                    dict[child.Name.LocalName] = ParseElement(child);
                return dict;
            }
        }

        public string Serialize(object obj)
        {
            var root = new XElement("Root");
            AddElements(root, obj);
            return new XDocument(root).ToString();
        }

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
