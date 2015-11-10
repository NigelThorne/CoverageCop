using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using JetBrains.Annotations;
using NAnt.Core;

namespace NCoverCop
{
    public class NCoverFileReader
    {
        private readonly INCoverXmlParser[] _parsers = {new NCoverXmlParserV1(), new NCoverXmlParserV2(), new OpenCoverXmlParser() };

        public NCoverResults Open(string coverageFile, Regex partOfPathToKeep)
        {
            IEnumerable<string> files = new FileFilter(coverageFile);
            var results = files.SelectMany(file => NCoverResultsFromFile(file, partOfPathToKeep)).ToArray();
            if (!results.Any())
            {
                throw new BuildException("The coverageFile specified does not exist");
            }
            return new NCoverResults(results);
        }

        private IEnumerable<INCoverNode> NCoverResultsFromFile(string coverageFile, Regex partOfPathToKeep)
        {
            Console.WriteLine($"Scanning: {coverageFile}");
            var reader = SelectReader(coverageFile);
            if (!reader.Exists(coverageFile)) return new INCoverNode[0];

            try
            {
                var document = new XmlDocument();
                document.LoadXml(reader.Read(coverageFile));
                return BestParser(document).ParseXmlDocument(document, partOfPathToKeep);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }

            return new INCoverNode[0];
        }

        private INCoverXmlParser BestParser(XmlNode document)
        {
            var node = document.SelectSingleNode("//coverage");
            if (node == null)
            {
                return new OpenCoverXmlParser(); //opencover ? 
            }
            var version = XmlNodeHelper.GetStringAttribute("profilerVersion", node);
            return int.Parse(version.Split('.')[0]) >= 2 ? _parsers[1] : _parsers[0];
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

    public class FileFilter : IEnumerable<string>
    {
        private readonly IEnumerable<string> _files;

        public FileFilter(string coverageFile)
        {
            _files = coverageFile.Contains('*') 
                ? GetMatchingFiles(coverageFile) 
                : new []{ coverageFile };
        }

        private IEnumerable<string> GetMatchingFiles([NotNull] string coverageFile)
        {
            var searchPattern = Path.GetFileName(coverageFile);
            if (string.IsNullOrWhiteSpace(searchPattern)) searchPattern = "*.*";
            var path = Path.GetDirectoryName(coverageFile);
            if(string.IsNullOrWhiteSpace(path)) path = Directory.GetCurrentDirectory();
            path = Path.GetFullPath(path);
            return Directory.GetFileSystemEntries(
                path: path, 
                searchPattern: searchPattern, 
                searchOption: SearchOption.TopDirectoryOnly);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _files.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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