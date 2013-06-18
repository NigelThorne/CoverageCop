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
        private string coverageFile;
        private string previousCoverageFile;
        private double minPercentage;
        private bool autoUpdate = true;
        private string sectionOfFilePathToCompareRegex = ".*";
        private bool skipLogging = false;

        [TaskAttribute("coverageFile", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string CoverageFile
        {
            get { return coverageFile; }
            set { coverageFile = value; }
        }

        [TaskAttribute("previousCoverageFile", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string PreviousCoverageFile
        {
            get { return previousCoverageFile; }
            set { previousCoverageFile = value; }
        }

        [TaskAttribute("minCoveragePercentage", Required = true)]
        public double MinPercentage
        {
            get { return ConvertToPercentage(minPercentage); }
            set { minPercentage = value; }
        }

        [TaskAttribute("autoUpdate")]
        public bool AutoUpdate
        {
            get { return autoUpdate; }
            set { autoUpdate = value; }
        }

        [TaskAttributeAttribute("sectionOfFilePathToCompareRegex")]
        public string SectionOfFilePathToCompareRegex
        {
            get { return sectionOfFilePathToCompareRegex; }
            set { sectionOfFilePathToCompareRegex = value; }
        }

        public void ExecuteInTesting()
        {
            skipLogging = true;
            this.ExecuteTask();
        }

        protected override void ExecuteTask()
        {
            try
            {
                NCoverFileReader reader = new NCoverFileReader();

                Threshold threshold =
                    new Threshold(
                        SafeOpen(reader, previousCoverageFile,
                                    new Regex(sectionOfFilePathToCompareRegex, RegexOptions.IgnoreCase)),
                        reader.Open(coverageFile,
                                    new Regex(sectionOfFilePathToCompareRegex, RegexOptions.IgnoreCase)),
                        MinPercentage);

                if (threshold.Passed)
                {
                    LLog(Level.Info, threshold.Message);
                    if (autoUpdate)
                    {
                        File.Copy(coverageFile, previousCoverageFile, true);
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
            if (skipLogging)
            {
                Console.WriteLine(info.ToString()+ ": " + message);
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