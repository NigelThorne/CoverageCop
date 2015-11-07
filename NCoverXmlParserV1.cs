using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace NCoverCop
{
    public class NCoverXmlParserV1 : INCoverXmlParser
    {
        public List<INCoverNode> ParseXmlDocument(XmlDocument results, Regex partOfPathToKeep)
        {
            var nodes = new List<INCoverNode>();
            foreach (XmlNode node in results.SelectNodes("//seqpnt"))
            {
                nodes.Add(ParseNode(node, partOfPathToKeep));
            }
            return nodes;
        }

        private static NCoverNode ParseNode(XmlNode node, Regex documentPathIgnoreMatcher)
        {
            return new NCoverNode(
                XmlNodeHelper.GetIntAttribute("line", node),
                XmlNodeHelper.GetIntAttribute("column", node),
                XmlNodeHelper.GetIntAttribute("endline", node),
                XmlNodeHelper.GetIntAttribute("endcolumn", node),
                XmlNodeHelper.GetStringAttribute("document", node),
                XmlNodeHelper.GetIntAttribute("visitcount", node),
                XmlNodeHelper.GetBoolAttribute("excluded", node),
                XmlNodeHelper.GetStringAttribute("name", node.ParentNode),
                XmlNodeHelper.GetIntAttribute("line", node.ParentNode.FirstChild),
                XmlNodeHelper.GetStringAttribute("class", node.ParentNode),
                documentPathIgnoreMatcher);
        }
    }
}