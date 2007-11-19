using System.Text.RegularExpressions;
using NUnit.Framework;


namespace NCoverCop.Tests
{
    [TestFixture]
    public class NCoverResultsTests 
    {
        private NCoverNode node1to1;
        private NCoverNode node2to2;
        private NCoverNode node4to4;
        private NCoverNode node5to5;
        private NCoverResults resultsAll;
        private NCoverResults resultsNone;
        private NCoverResults results1to1n4to4;

        [SetUp]
        public  void SetUp()
        {
            node1to1 = new NCoverNode(1, 0, 1, 1, "doc1", 0, false, new Regex(".*"));
            node2to2 = new NCoverNode(2, 0, 2, 1, "doc1", 0, false, new Regex(".*"));
            node4to4 = new NCoverNode(4, 0, 4, 1, "doc1", 0, false, new Regex(".*"));
            node5to5 = new NCoverNode(5, 0, 5, 1, "doc1", 0, false, new Regex(".*"));

            resultsNone = new NCoverResults(new NCoverNode[] {});
            resultsAll = new NCoverResults(new NCoverNode[] { node1to1, node2to2, node4to4, node5to5 });
            results1to1n4to4 = new NCoverResults(new NCoverNode[] { node1to1, node4to4 });

        }

        [Test]
        public void ReportNewUntestedCode_DisplaysUntestedLines()
        {
            Assert.AreEqual("Line 2-2 in doc1\nLine 5-5 in doc1\n", resultsAll.ReportNewUntestedCode(results1to1n4to4));
            Assert.AreEqual("", results1to1n4to4.ReportNewUntestedCode(resultsAll));
        }

        [Test]
        public void ReportNewUntestedCode_JoinsBlocksOfuntestedCodeIntoASingleEntry()
        {
            Assert.AreEqual("Line 1-2 in doc1\nLine 4-5 in doc1\n", resultsAll.ReportNewUntestedCode(resultsNone));
        }

    }
}