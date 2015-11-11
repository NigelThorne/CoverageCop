using System;
using NUnit.Framework;

namespace NCoverCop.Tests
{
    [TestFixture]
    public class NCoverUatTests
    {
        [SetUp]
        public void SetUp()
        {
            _task = new NCoverCopTask
            {
                PreviousCoveragePath = @"./new/",
                CoverageFile = @"./old/NCoverResults.xml",
                MinPercentage = 40,
                SectionOfFilePathToCompareRegex = @"trunk\\.*"
            };
        }

        private NCoverCopTask _task;

        [Test]
        public void Execute_action_condition()
        {
            _task.PreviousCoveragePath = @"./new/";
            _task.CoverageFile = @"./old/NCoverResults.xml";
            try
            {
                _task.ExecuteInTesting();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        [Test]
        public void Execute2_action_condition()
        {
            _task.CoverageFile = @"./new/NCoverResults.xml";
            _task.PreviousCoveragePath = @"./old/";
            try
            {
                _task.ExecuteInTesting();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        [Test]
        public void Execute3_action_condition()
        {
            _task.PreviousCoveragePath =
                @"http://teamcity:8000/guestAuth/app/rest/builds/buildType:(id:bt68),status:SUCCESS/artifacts/files/ncover/All/Full/";
            _task.CoverageFile =
                @"http://teamcity:8000/guestAuth/app/rest/builds/buildType:(id:bt68)/artifacts/files/ncover/All/Full/merged.xml";
            _task.SectionOfFilePathToCompareRegex = @"Bond\\.*";
            try
            {
                _task.ExecuteInTesting();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }
    }
}