using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KS.Collections
{
    public static class Xml
    {
        public static string ReadTag(string xml, string tag)
        {
            var start_tag = $"<{tag}";
            var end_tag = $"</{tag}";

            var startIndex = xml.IndexOf(start_tag);
            if (startIndex < 0)
                throw new Exception($"Tag {tag} not found");

            startIndex = xml.IndexOf(">", startIndex) + 1;      // Removing tag in de index
            var endIndex = xml.IndexOf(end_tag, startIndex);    // Find end tag;

            var value = xml.Substring(startIndex, endIndex - startIndex).Replace("\r\n", "");

            return value;
        }

        /// <summary>
        /// Read all tags in the root tag.
        /// </summary>
        /// <param name="xml">Xml file</param>
        /// <param name="root">root tag</param>
        /// <param name="tag">tag to read</param>
        /// <returns>Tag value</returns>
        public static List<string> ReadTags(string xml, string root, string tag)
        {
            var xmlparse = ReadTag(xml, root);
            var res = new List<string>();

            var start_tag = $"<{tag}";
            var end_tag = $"</{tag}";


            MatchCollection matches = Regex.Matches(xmlparse, start_tag);
            foreach (Match match in matches)
            {
                var startIndex = xmlparse.IndexOf(">", match.Index) + 1;
                var endIndex = xmlparse.IndexOf(end_tag, startIndex);
                var value = xmlparse.Substring(startIndex, endIndex - startIndex);
                res.Add(value);
            }

            return res;
        }
    }
}
