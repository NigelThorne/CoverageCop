using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace NCoverCop
{
    public class NCoverXmlParserV2 : INCoverXmlParser
    {
        private readonly Dictionary<int, string> documents = new Dictionary<int, string>();

        public List<INCoverNode> ParseXmlDocument(XmlDocument results, Regex partOfPathToKeep)
        {
            foreach (XmlNode node in results.SelectNodes("//doc"))
            {
                if (!XmlNodeHelper.GetBoolAttribute("excluded", node)) 
                    documents[XmlNodeHelper.GetIntAttribute("id", node)] = XmlNodeHelper.GetStringAttribute("url", node);
            }

            List<INCoverNode> nodes = new List<INCoverNode>();
            foreach (XmlNode node in results.SelectNodes("//seqpnt"))
            {
                nodes.Add(ParseNode(node, partOfPathToKeep));
            }
            return nodes;
        }

        private NCoverNode ParseNode(XmlNode node, Regex documentPathIgnoreMatcher)
        {
            return new NCoverNode(
                XmlNodeHelper.GetIntAttribute("l", node),
                XmlNodeHelper.GetIntAttribute("c", node),
                XmlNodeHelper.GetIntAttribute("el", node),
                XmlNodeHelper.GetIntAttribute("ec", node),
                documents[XmlNodeHelper.GetIntAttribute("doc", node)],
                XmlNodeHelper.GetIntAttribute("vc", node),
                XmlNodeHelper.GetBoolAttribute("ex", node),
                XmlNodeHelper.GetStringAttribute("name", node.ParentNode),
                XmlNodeHelper.GetIntAttribute("l", node.ParentNode.FirstChild),
                XmlNodeHelper.GetStringAttribute("name", node.ParentNode.ParentNode),
                documentPathIgnoreMatcher);
        }
    }
}