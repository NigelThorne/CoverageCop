using System;
using System.Text.RegularExpressions;

namespace NCoverCop
{
    public class NCoverNode : INCoverNode
    {
        private readonly Regex documentPathIgnoreMatcher;
        private readonly int visitCount;

        public NCoverNode(int line, int column, int endLine, int endColumn, string document, int visitCount,
            bool excluded, string method, int firstLineOfMethod, string klass, Regex documentPathIgnoreMatcher)
        {
            Line = line;
            Column = column;
            EndLine = endLine;
            EndColumn = endColumn;
            Document = documentPathIgnoreMatcher.Match(document).Value;
            this.visitCount = visitCount;
            IsExcluded = excluded;
            Method = method;
            Klass = klass;
            this.documentPathIgnoreMatcher = documentPathIgnoreMatcher;
            MethodLineOffset = line - firstLineOfMethod;
        }

        public override string ToString()
        {
            return string.Format("{4} {3} \t\t Line {0}-{1} in {2}", Line, EndLine, Document,
                Method.Substring(Method.LastIndexOf(".") + 1), Klass.Substring(Klass.LastIndexOf(".") + 1));
        }

        #region INCoverNode Members

        public bool IsExcluded { get; }

        public bool IsVisited => visitCount > 0;

        public int Line { get; }

        public int Column { get; }

        public int EndLine { get; }

        public int EndColumn { get; }

        public string Document { get; }

        public string Method { get; }

        public int MethodLineOffset { get; }

        public string Klass { get; }

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
            if (!node.Follows(this)) throw new NotImplementedException();

            return new NCoverNode(Line, Column, node.EndLine, node.EndColumn, Document, visitCount, IsExcluded, Method,
                MethodLineOffset + Line, Klass, documentPathIgnoreMatcher);
        }

        #endregion
    }
}