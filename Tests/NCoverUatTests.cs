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
                PreviousCoverageFile = @"./new_NCoverResults.xml",
                CoverageFile = @"./old_NCoverResults.xml",
                MinPercentage = 40,
                SectionOfFilePathToCompareRegex = @"trunk\\.*"
            };
        }

        private NCoverCopTask _task;

        [Test]
        public void Execute_action_condition()
        {
            _task.PreviousCoverageFile = @"./new_NCoverResults.xml";
            _task.CoverageFile = @"./old_NCoverResults.xml";
            try
            {
                _task.ExecuteInTesting();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception e)
// ReSharper restore EmptyGeneralCatchClause
            {
                Console.Write(e.Message);
            }
        }

        [Test]
        public void Execute2_action_condition()
        {
            _task.CoverageFile = @"./new_NCoverResults.xml";
            _task.PreviousCoverageFile = @"./old_NCoverResults.xml";
            try
            {
                _task.ExecuteInTesting();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception e)
// ReSharper restore EmptyGeneralCatchClause
            {
                Console.Write(e.Message);
            }
        }

        [Test]
        public void Execute3_action_condition()
        {
            _task.PreviousCoverageFile =
                @"http://teamcity:8000/guestAuth/app/rest/builds/buildType:(id:bt68),status:SUCCESS/artifacts/files/ncover/All/Full/merged.xml";
            _task.CoverageFile =
                @"http://teamcity:8000/guestAuth/app/rest/builds/buildType:(id:bt68)/artifacts/files/ncover/All/Full/merged.xml";
            _task.SectionOfFilePathToCompareRegex = @"Bond\\.*";
            try
            {
                _task.ExecuteInTesting();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception e)
// ReSharper restore EmptyGeneralCatchClause
            {
                Console.Write(e.Message);
            }
        }
    }
}