using System;
using NUnit.Framework;

namespace NCoverCop.Tests
{
    [TestFixture]
    public class OpenCoverUatTests
    {
        [SetUp]
        public void SetUp()
        {
            _task = new NCoverCopTask
            {
                PreviousCoveragePath = @"./old/",
                CoverageFile = @"./new/OpenCoverResults.xml",
                MinPercentage = 40,
                SectionOfFilePathToCompareRegex = @"bond-rx\\.*"
            };
        }

        private NCoverCopTask _task;

        [Test]
        public void Execute_Task_WithSuccessfulCoverageFiles()
        {
            Assert.IsFalse(_task.ExecuteInTesting(), "Should have complained about drop in coverage.");
        }
    }
}