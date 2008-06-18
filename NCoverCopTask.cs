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

        protected override void ExecuteTask()
        {
            try
            {
                if (!File.Exists(coverageFile))
                    throw new BuildException("The coverageFile specified does not exist");

                NCoverFileParser parser = new NCoverFileParser();

                Threshold threshold =
                    new Threshold(parser.Open(previousCoverageFile, new Regex(sectionOfFilePathToCompareRegex, RegexOptions.IgnoreCase)), parser.Open(coverageFile, new Regex(sectionOfFilePathToCompareRegex, RegexOptions.IgnoreCase)), MinPercentage);

                if (threshold.Passed)
                {
                    Log(Level.Info, threshold.Message);                    
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

        internal static double ConvertToPercentage(double value)
        {
            return value/100.0;
        }
    }
}