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
                PreviousCoverageFile = @"./old_OpenCoverResults.xml",
                CoverageFile = @"./new_OpenCoverResults.xml",
                AutoUpdate = false,
                MinPercentage = 40,
                SectionOfFilePathToCompareRegex = @"bond-rx\\.*"
            };
        }

        private NCoverCopTask _task;

        [Test]
        public void Execute_Task_WithSuccessfulCoverageFiles()
        {
            try
            {
                _task.ExecuteInTesting();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            Assert.Fail("Should have complained about drop in coverage.");
        }
    }
}