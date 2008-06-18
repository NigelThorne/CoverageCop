using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace NCoverCop
{
    public interface INCoverXmlParser
    {
        List<INCoverNode> ParseXmlDocument(XmlDocument results, Regex partOfPathToKeep);
    }
}