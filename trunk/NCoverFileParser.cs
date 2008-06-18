using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace NCoverCop
{
    public class NCoverFileParser
    {
        public NCoverNode ParseNode(XmlNode node, Regex documentPathIgnoreMatcher)
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

        public NCoverResults Open(string coverageFile, Regex partOfPathToKeep)
        {
            List<INCoverNode> nodes = new List<INCoverNode>();
            XmlDocument results = new XmlDocument();
            if (File.Exists(coverageFile))
            {
                try
                {
                    results.LoadXml(File.ReadAllText(coverageFile));
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(ex.Message);
                }

                foreach (XmlNode node in results.SelectNodes("//seqpnt"))
                {
                    nodes.Add(this.ParseNode(node, partOfPathToKeep));
                }
            }
            return new NCoverResults(nodes);
        }

    }
}