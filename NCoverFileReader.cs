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
        private readonly INCoverXmlParser[] parsers = {new NCoverXmlParserV1(), new NCoverXmlParserV2(), new OpenCoverXmlParser() };

        public NCoverResults Open(string coverageFile, Regex partOfPathToKeep)
        {
            var reader = SelectReader(coverageFile);
            if (!reader.Exists(coverageFile))
            {
                throw new BuildException("The coverageFile specified does not exist");
            }

            try
            {
                //System.Diagnostics.Debugger.Break();

                var document = new XmlDocument();
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
            var node = document.SelectSingleNode("//coverage");
            if (node == null)
            {
                return new OpenCoverXmlParser(); //opencover ? 
            }
            var version = XmlNodeHelper.GetStringAttribute("profilerVersion", node);
            return int.Parse(version.Split('.')[0]) >= 2 ? parsers[1] : parsers[0];
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
            var httpRequest = (HttpWebRequest) WebRequest.Create(location);

            httpRequest.Timeout = 1000*60*5; // 5 minutes
            httpRequest.UserAgent = "Code Sample Web Client";

            var webResponse = (HttpWebResponse) httpRequest.GetResponse();
            var responseStream = new StreamReader(webResponse.GetResponseStream());

            return responseStream.ReadToEnd();
        }

        public bool Exists(string location)
        {
            try
            {
                var request = WebRequest.Create(location) as HttpWebRequest;
                request.Method = "HEAD";
                var response = request.GetResponse() as HttpWebResponse;
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