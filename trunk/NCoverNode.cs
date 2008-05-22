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
        private readonly string method;
        private readonly int methodLineOffset;
        private readonly string klass;
        private readonly Regex documentPathIgnoreMatcher;
        private readonly int line;
        private readonly int visitCount;
        private string[] split;

        public NCoverNode(XmlNode node, Regex documentPathIgnoreMatcher):
            this(
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
            documentPathIgnoreMatcher)
        {}

        public NCoverNode(int line, int column, int endLine, int endColumn, string document, int visitCount, bool excluded, string method, int firstLineOfMethod, string klass, Regex documentPathIgnoreMatcher)
        {
            this.line = line;
            this.column = column;
            this.endLine = endLine;
            this.endColumn = endColumn;
            this.document = documentPathIgnoreMatcher.Match(document).Value;
            this.visitCount = visitCount;
            this.excluded = excluded;
            this.method = method;
            this.klass = klass;
            this.documentPathIgnoreMatcher = documentPathIgnoreMatcher;
            this.methodLineOffset = line - firstLineOfMethod;
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

        public string Method
        {
            get { return method; }
        }

        public int MethodLineOffset
        {
            get { return methodLineOffset; }
        }

        public string Klass
        {
            get { return klass; }
        }

        public bool Matches(INCoverNode ncoverNode)
        {
            return
                ncoverNode.Method == Method &&
                ncoverNode.MethodLineOffset == MethodLineOffset &&
                ncoverNode.Klass == Klass;
        }

        public bool Follows(INCoverNode node)
        {
            return node.Document == Document &&
                   (node.EndLine == Line || node.EndLine + 1 == Line) && node.Method == Method && node.Klass == Klass;
        }

        public INCoverNode ExtendWith(INCoverNode node)
        {
            if (!node.Follows(this))
                throw new NotImplementedException();
            else
            {
                return new NCoverNode(Line, Column, node.EndLine, node.EndColumn, Document, visitCount, excluded, method, methodLineOffset + line, klass, documentPathIgnoreMatcher);
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{4} {3} \t\t Line {0}-{1} in {2}", Line, EndLine, document, method.Substring(method.LastIndexOf(".") + 1), klass.Substring(klass.LastIndexOf(".") + 1));
        }
    }

    public class XmlNodeHelper
    {
        public static int GetIntAttribute(string name, XmlNode node)
        {
            return node.Attributes[name] != null ? int.Parse(node.Attributes[name].Value) : 0;
        }

        public static string GetStringAttribute(string name, XmlNode node)
        {
            return node.Attributes[name] != null ? node.Attributes[name].Value : "";
        }

        public static bool GetBoolAttribute(string name, XmlNode node)
        {
            return node.Attributes[name] != null && node.Attributes[name].Value != "false";
        }
    }
}