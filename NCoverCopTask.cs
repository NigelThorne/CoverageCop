using System;
using System.IO;
using System.Text.RegularExpressions;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace NCoverCop
{
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

        [TaskAttribute("autoUpdate")]
        public bool AutoUpdate { get; set; } = true;

        [TaskAttribute("sectionOfFilePathToCompareRegex")]
        public string SectionOfFilePathToCompareRegex { get; set; } = ".*";

        public void ExecuteInTesting()
        {
            _skipLogging = true;
            ExecuteTask();
        }

        protected override void ExecuteTask()
        {
            try
            {
                var reader = new NCoverFileReader();

                var threshold =
                    new Threshold(
                        SafeOpen(reader, PreviousCoverageFile,
                            new Regex(SectionOfFilePathToCompareRegex, RegexOptions.IgnoreCase)),
                        reader.Open(CoverageFile,
                            new Regex(SectionOfFilePathToCompareRegex, RegexOptions.IgnoreCase)),
                        MinPercentage);

                if (threshold.Passed)
                {
                    LLog(Level.Info, threshold.Message);
                    if (AutoUpdate)
                    {
                        File.Copy(CoverageFile, PreviousCoverageFile, true);
                    }
                }
                else
                {
                    throw new BuildException(threshold.Message);
                }
            }
            catch (Exception e)
            {
                throw new BuildException(e.Message);
            }
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