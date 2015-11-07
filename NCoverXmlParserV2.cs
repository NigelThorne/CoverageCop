using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using JetBrains.Annotations;

namespace NCoverCop
{
    public static class XmlDocumentExtensionMethods
    {
        public static IEnumerable<XmlNode> SafeSelectNodes(this XmlDocument doc, string xpath)
        {
            return (doc.SelectNodes(xpath) ?? (IEnumerable) new XmlNode[0]).Cast<XmlNode>();
        }
    }

    public class OpenCoverXmlParser : INCoverXmlParser
    {
        private readonly Dictionary<int, string> _documents = new Dictionary<int, string>();
        public List<INCoverNode> ParseXmlDocument( XmlDocument results, Regex partOfPathToKeep)
        {
            foreach (var node in results.SafeSelectNodes("//File"))
            {
                _documents[XmlNodeHelper.GetIntAttribute("uid", node)] = XmlNodeHelper.GetStringAttribute("fullPath", node);
            }

            return (results.SafeSelectNodes("//SequencePoint")
                .Select(node => ParseNode(node, partOfPathToKeep))).ToList();
        }
        private INCoverNode ParseNode(XmlNode sequenceNode, Regex documentPathIgnoreMatcher)
        {
            var methodNode = sequenceNode?.ParentNode?.ParentNode;
            var classNode = methodNode?.ParentNode.ParentNode;
            return new NCoverNode(
                XmlNodeHelper.GetIntAttribute("sl", sequenceNode),
                XmlNodeHelper.GetIntAttribute("sc", sequenceNode),
                XmlNodeHelper.GetIntAttribute("el", sequenceNode),
                XmlNodeHelper.GetIntAttribute("ec", sequenceNode),
                _documents[XmlNodeHelper.GetIntAttribute("fileid", sequenceNode)],
                XmlNodeHelper.GetIntAttribute("vc", sequenceNode),
                false, // excluded
                methodNode?.SelectSingleNode("Name")?.InnerText,
                XmlNodeHelper.GetIntAttribute("sl", sequenceNode?.ParentNode?.FirstChild),
                classNode?.SelectSingleNode("FullName")?.InnerText,
                documentPathIgnoreMatcher);
        }
    }

    public class NCoverXmlParserV2 : INCoverXmlParser
    {
        private readonly Dictionary<int, string> documents = new Dictionary<int, string>();

        public List<INCoverNode> ParseXmlDocument(XmlDocument results, Regex partOfPathToKeep)
        {
            foreach (var node in results.SafeSelectNodes("//doc").Where(node => !XmlNodeHelper.GetBoolAttribute("excluded", node)))
            {
                documents[XmlNodeHelper.GetIntAttribute("id", node)] = XmlNodeHelper.GetStringAttribute("url", node);
            }

            return (results.SafeSelectNodes("//seqpnt").Select(node => ParseNode(node, partOfPathToKeep))).ToList();
        }

        private INCoverNode ParseNode(XmlNode node, Regex documentPathIgnoreMatcher)
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