using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using JetBrains.Annotations;

namespace NCoverCop
{
    public interface INCoverXmlParser
    {
        List<INCoverNode> ParseXmlDocument([NotNull] XmlDocument results, [NotNull]  Regex partOfPathToKeep);
    }
}