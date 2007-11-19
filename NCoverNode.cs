using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace NCoverCop
{
    public class NCoverNode : INCoverNode
    {
        private readonly int column;
        private readonly string document;
        private readonly int endColumn;
        private readonly int endLine;
        private readonly bool excluded;
        private readonly Regex documentPathIgnoreMatcher;
        private readonly int line;
        private readonly int visitCount;

        public NCoverNode(XmlNode node, Regex documentPathIgnoreMatcher):
            this(
            GetIntAttribute("line", node),
            GetIntAttribute("column", node),
            GetIntAttribute("endline", node),
            GetIntAttribute("endcolumn", node),
            GetStringAttribute("document", node),
            GetIntAttribute("visitcount", node),
            GetBoolAttribute("excluded", node),
            documentPathIgnoreMatcher)
        {}

        public NCoverNode(int line, int column, int endLine, int endColumn, string document, int visitCount, bool excluded, Regex documentPathIgnoreMatcher)
        {
            this.line = line;
            this.column = column;
            this.endLine = endLine;
            this.endColumn = endColumn;
            this.document = documentPathIgnoreMatcher.Match(document).Value;
            this.visitCount = visitCount;
            this.excluded = excluded;
            this.documentPathIgnoreMatcher = documentPathIgnoreMatcher;
        }

        #region INCoverNode Members

        public bool IsExcluded
        {
            get { return excluded; }
        }

        public bool IsVisited
        {
            get { return visitCount>0; }
        }

        public int Line
        {
            get { return line; }
        }

        public int Column
        {
            get { return column; }
        }

        public int EndLine
        {
            get { return endLine; }
        }

        public int EndColumn
        {
            get { return endColumn; }
        }

        public string Document
        {
            get { return document; }
        }

        public bool Matches(INCoverNode ncoverNode)
        {
            return
                ncoverNode.Line == Line &&
                ncoverNode.Column == Column &&
                ncoverNode.EndLine == EndLine &&
                ncoverNode.EndColumn == EndColumn &&
                ncoverNode.Document == Document;
        }

        public bool Follows(INCoverNode node)
        {
            return node.Document == Document &&
                   (node.EndLine == Line || node.EndLine + 1 == Line);
        }

        public INCoverNode ExtendWith(INCoverNode node)
        {
            if (!node.Follows(this))
                throw new NotImplementedException();
            else
            {
                return new NCoverNode(Line, Column, node.EndLine, node.EndColumn, Document, visitCount, excluded, documentPathIgnoreMatcher);
            }
        }

        #endregion

        private static int GetIntAttribute(string name, XmlNode node)
        {
            return node.Attributes[name] != null ? int.Parse(node.Attributes[name].Value) : 0;
        }

        private static string GetStringAttribute(string name, XmlNode node)
        {
            return node.Attributes[name] != null ? node.Attributes[name].Value : "";
        }

        private static bool GetBoolAttribute(string name, XmlNode node)
        {
            return node.Attributes[name] != null && node.Attributes[name].Value != "false";
        }

        public override string ToString()
        {
            return string.Format("Line {0}-{1} in {2}", Line, EndLine, document);
        }
    }
}