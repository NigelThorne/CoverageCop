using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using NAnt.Core;

namespace NCoverCop
{
    public class NCoverFileReader
    {
        readonly INCoverXmlParser[] parsers = new INCoverXmlParser[] { new NCoverXmlParserV1(), new NCoverXmlParserV2() };

        public NCoverResults Open(string coverageFile, Regex partOfPathToKeep)
        {
            IFileReader reader = SelectReader(coverageFile);
            if (!reader.Exists(coverageFile))
            {
                throw new BuildException("The coverageFile specified does not exist");
            }

            try
            {
                //System.Diagnostics.Debugger.Break();

                XmlDocument document = new XmlDocument();
                document.LoadXml(reader.Read(coverageFile));
                return new NCoverResults(BestParser(document).ParseXmlDocument(document, partOfPathToKeep));
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }

            return new NCoverResults(new INCoverNode[0]);
        }

        private INCoverXmlParser BestParser(XmlNode document)
        {
            XmlNode node = document.SelectSingleNode("//coverage");
            string version = XmlNodeHelper.GetStringAttribute("profilerVersion", node);
            return Int32.Parse(version.Split('.')[0]) >= 2 ? parsers[1] : parsers[0];
        }

        private static bool IsUrl(string location)
        {
            return location.StartsWith(@"http://", true, CultureInfo.InvariantCulture) || 
                location.StartsWith(@"https://", true, CultureInfo.InvariantCulture) || 
                location.StartsWith(@"file://", true, CultureInfo.InvariantCulture);
        }

        private IFileReader SelectReader(string location)
        {
            return IsUrl(location) ? (IFileReader) new WebFileReader() : new LocalFileReader();
        }

        
    }

    public interface IFileReader
    {
        string Read(string location);
        bool Exists(string location);
    }

    public class WebFileReader : IFileReader
    {
        public string Read(string location)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(location);

            httpRequest.Timeout = 1000 * 60 * 5;     // 5 minutes
            httpRequest.UserAgent = "Code Sample Web Client";

            HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader responseStream = new StreamReader(webResponse.GetResponseStream());

            return responseStream.ReadToEnd();
        }

        public bool Exists(string location)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(location) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                //Any exception will returns false.
                return false;
            }
        }
    }

    public class LocalFileReader : IFileReader
    {
        public string Read(string location)
        {
            return File.ReadAllText(location);
        }

        public bool Exists(string location)
        {
            return File.Exists(location);
        }
    }
}