using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace NCoverCop
{
    public class NCoverFileReader
    {
        readonly INCoverXmlParser[] parsers = new INCoverXmlParser[] { new NCoverXmlParserV1(), new NCoverXmlParserV2() };

        public NCoverResults Open(string coverageFile, Regex partOfPathToKeep)
        {
            if (File.Exists(coverageFile))
            {
                try
                {
                    //System.Diagnostics.Debugger.Break();

                    XmlDocument document = new XmlDocument();
                    document.LoadXml(File.ReadAllText(coverageFile));
                    return new NCoverResults(BestParser(document).ParseXmlDocument(document, partOfPathToKeep));
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(ex.Message);
                }

            }
            return new NCoverResults(new INCoverNode[0]);
        }

        private INCoverXmlParser BestParser(XmlNode document)
        {
            XmlNode node = document.SelectSingleNode("//coverage");
            string version = XmlNodeHelper.GetStringAttribute("profilerVersion", node);
            return Int32.Parse(version.Split('.')[0]) >= 2 ? parsers[1] : parsers[0];
        }
    }
}