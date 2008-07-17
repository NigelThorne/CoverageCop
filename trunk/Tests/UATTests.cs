using System.Xml;
using NAnt.Core;
using NUnit.Framework;


namespace NCoverCop.Tests
{
    [TestFixture]
    public class UATTests
    {
        private NCoverCopTask task;

        [SetUp]
        public void SetUp()
        {
            task = new NCoverCopTask();
            this.task.PreviousCoverageFile = @"C:\Builds\3rdParty\ncovercop\trunk\new_NCoverResults.xml";
            this.task.CoverageFile = @"C:\Builds\3rdParty\ncovercop\trunk\old_NCoverResults.xml";
            this.task.AutoUpdate = false;
            this.task.MinPercentage = 40;
            this.task.SectionOfFilePathToCompareRegex = @"trunk\\.*";

        }

        [Test]
        public void Execute_action_condition()
        {
            this.task.PreviousCoverageFile = @"C:\Builds\3rdParty\ncovercop\trunk\new_NCoverResults.xml";
            this.task.CoverageFile = @"C:\Builds\3rdParty\ncovercop\trunk\old_NCoverResults.xml";
            task.ExecuteInTesting();
        }
        
        [Test]
        public void Execute2_action_condition()
        {
            this.task.CoverageFile = @"C:\Builds\3rdParty\ncovercop\trunk\new_NCoverResults.xml";
            this.task.PreviousCoverageFile = @"C:\Builds\3rdParty\ncovercop\trunk\old_NCoverResults.xml";
            task.ExecuteInTesting();
        }
    }
}