using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReactivePlayer.Core
{
    internal static class XElementExtensions
    {
        public static bool ParseBoolean(XElement track, string keyValue)
        {
            return (from keyNode in track.Descendants("key")
                    where keyNode.Value == keyValue
                    select (keyNode.NextNode as XElement).Name).FirstOrDefault() == "true";
        }

        public static string ParseStringValue(XElement track, string keyValue)
        {
            return (from key in track.Descendants("key")
                    where key.Value == keyValue
                    select (key.NextNode as XElement).Value).FirstOrDefault();
        }

        public static long ParseLongValue(XElement track, string keyValue)
        {
            var stringValue = ParseStringValue(track, keyValue);
            return long.Parse(stringValue);
        }

        public static int? ParseNullableIntValue(XElement track, string keyValue)
        {
            var stringValue = ParseStringValue(track, keyValue);
            return String.IsNullOrEmpty(stringValue) ? (int?)null : int.Parse(stringValue);
        }

        public static DateTime? ParseNullableDateValue(XElement track, string keyValue)
        {
            var stringValue = ParseStringValue(track, keyValue);
            return String.IsNullOrEmpty(stringValue) ? (DateTime?)null : DateTime.SpecifyKind(DateTime.Parse(stringValue, CultureInfo.InvariantCulture), DateTimeKind.Utc).ToLocalTime();
        }

        public static IDictionary<string, string> XDictToDictionary(this XElement xDict)
        {
            var isLocation = xDict.Descendants().Any(d => d.Value == @"file://localhost/D:/Music/P!nk%20-%20Sober.mp3");
            var dict = new Dictionary<string, string>();
            var descs = xDict
                .Elements()
                .InDocumentOrder()
                .ToList();
            while (descs.Any())
            {
                var xKey = descs.FirstOrDefault();
                if (xKey.Name == "key")
                {
                    descs.Remove(xKey);
                    var xValue = descs.FirstOrDefault();
                    if (xValue.Name != "key")
                    {
                        string value = xValue.Value;

                        switch (xValue.Name.LocalName)
                        {
                            case "true":
                            case "false":
                                value = xValue.Name.LocalName;
                                break;
                        }

                        dict[xKey.Value] = value;
                        descs.Remove(xValue);
                    }
                    else
                    {
                        dict[xKey.Value] = null;
                    }
                }
                else
                {
                    throw new Exception($"{xKey.Name} is not a key");
                }
            }

            return dict;
        }
    }
}