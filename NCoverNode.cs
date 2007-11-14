using System.Xml;


namespace NCoverCop
{
    public class NCoverNode : INCoverNode
    {
        private readonly XmlNode node;
        private readonly int line;
        private readonly int column;
        private readonly int endLine;
        private readonly int endColumn;
        private readonly string document;

        public NCoverNode(XmlNode node)
        {
            this.node = node;
            line = GetIntAttribute("line");
            endLine = GetIntAttribute("endline");
            column = GetIntAttribute("column");
            endColumn = GetIntAttribute("endcolumn");
            document = GetStringAttribute("document");
        }

        public NCoverNode(int line, int column, int endLine, int endColumn, string document)
        {
            this.line = line;
            this.column = column;
            this.endLine = endLine;
            this.endColumn = endColumn;
            this.document = document;
        }

        private int GetIntAttribute(string name)
        {
            return node.Attributes[name] != null ? int.Parse(node.Attributes[name].Value) : 0;
        }

        private string GetStringAttribute(string name)
        {
            return node.Attributes[name] != null ? node.Attributes[name].Value : "";
        }

        public bool IsExcluded
        {
            get { return node.Attributes["excluded"] != null && node.Attributes["excluded"].Value != "false"; }
        }

        public bool IsVisited
        {
            get { return (node.Attributes["visitcount"] != null && int.Parse(node.Attributes["visitcount"].Value) > 0); }
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
                (node.Line == EndLine || node.Line == EndLine+1);
        }

        public INCoverNode ExtendWith(INCoverNode node)
        {
            if(!node.Follows(this))
                throw new System.NotImplementedException();
            else
            {
                return new NCoverNode(Line,Column,node.EndLine,node.EndColumn,Document);
            }
        }

        public override string ToString()
        {
            return string.Format("Line {0}-{1} in {2}", Line, EndLine, document);
        }
    }
}