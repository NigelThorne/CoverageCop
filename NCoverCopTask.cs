using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace NCoverCop
{
    public class ExecutionResult
    {
        public string Message;
        public bool Passed;
    }

    [TaskName("ncoverCop")]
    public class NCoverCopTask : Task
    {
        private double _minPercentage;
        private bool _skipLogging;

        [TaskAttribute("coverageFile", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string CoverageFile { get; set; }

        [TaskAttribute("previousCoverageFile", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string PreviousCoverageFile { get; set; }

        [TaskAttribute("minCoveragePercentage", Required = true)]
        public double MinPercentage
        {
            get { return ConvertToPercentage(_minPercentage); }
            set { _minPercentage = value; }
        }

        [Obsolete("feature removed")]
        [TaskAttribute("autoUpdate")]
        public bool AutoUpdate { get; set; } = true;

        [TaskAttribute("sectionOfFilePathToCompareRegex")]
        public string SectionOfFilePathToCompareRegex { get; set; } = ".*";

        public bool ExecuteInTesting()
        {
            _skipLogging = true;
            var threshold = InnerExecute();
            Console.WriteLine(threshold.Message);
            return threshold.Passed;
        }

        protected override void ExecuteTask()
        {
            Threshold threshold = null;
            try
            {
                threshold = InnerExecute();
            }
            catch (Exception e)
            {
                throw new BuildException(e.Message, e);
            }

            if (threshold?.Passed ?? false)
            {
                LLog(Level.Info, threshold.Message);
            }
            else
            {
                throw new BuildException(threshold?.Message??"");
            }
        }

        private Threshold InnerExecute()
        {
            var reader = new NCoverFileReader();

            return new Threshold(
                    SafeOpen(reader, PreviousCoverageFile,
                        new Regex(SectionOfFilePathToCompareRegex, RegexOptions.IgnoreCase)),
                    reader.Open(CoverageFile,
                        new Regex(SectionOfFilePathToCompareRegex, RegexOptions.IgnoreCase)),
                    MinPercentage);
        }

        private NCoverResults SafeOpen(NCoverFileReader reader, string aCoverageFile, Regex partOfPathToKeep)
        {
            try
            {
                return reader.Open(aCoverageFile, partOfPathToKeep);
            }
            catch (Exception)
            {
                return new NCoverResults(new INCoverNode[0]);
            }
        }

        private void LLog(Level info, string message)
        {
            if (_skipLogging)
            {
                Console.WriteLine(info + ": " + message);
                return;
            }
            Log(info, message);
        }

        internal static double ConvertToPercentage(double value)
        {
            return value/100.0;
        }
    }
}