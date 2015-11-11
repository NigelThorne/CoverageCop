using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        [TaskAttribute("previousCoveragePath", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string PreviousCoveragePath { get; set; }

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
            var partOfPathToKeep = new Regex(SectionOfFilePathToCompareRegex, RegexOptions.IgnoreCase);

            IEnumerable<string> currentCoverageFiles = new FileFilter(CoverageFile);
            var previousCoverageFiles = currentCoverageFiles.Select(Path.GetFileName).Select(f => Path.Combine(PreviousCoveragePath, f));

            return new Threshold(
                    SafeRead(reader, previousCoverageFiles, partOfPathToKeep),
                    reader.Read(currentCoverageFiles, partOfPathToKeep),
                    MinPercentage);
        }

        private static NCoverResults SafeRead(NCoverFileReader reader, IEnumerable<string> coverageFiles, Regex partOfPathToKeep)
        {
            try
            {
                return reader.Read(coverageFiles, partOfPathToKeep);
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