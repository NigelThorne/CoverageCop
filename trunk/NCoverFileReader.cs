using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace NCoverCop
{
    public class NCoverFileReader
    {
        readonly INCoverXmlParser[] parsers = new INCoverXmlParser[] { new NCoverXmlParserV1() };

        public NCoverResults Open(string coverageFile, Regex partOfPathToKeep)
        {
            if (File.Exists(coverageFile))
            {
                try
                {
                    XmlDocument results = new XmlDocument();
                    results.LoadXml(File.ReadAllText(coverageFile));
                    return new NCoverResults(parsers[0].ParseXmlDocument(results, partOfPathToKeep));
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(ex.Message);
                }

            }
            return new NCoverResults(new INCoverNode[0]);
        }

    }

    public class NCoverXmlParserV1 : INCoverXmlParser
    {
        public List<INCoverNode> ParseXmlDocument(XmlDocument results, Regex partOfPathToKeep)
        {
            List<INCoverNode> nodes = new List<INCoverNode>();
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