using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NCoverCop.Tests
{
    [TestFixture]
    public class NCoverResultsTests
    {
        [SetUp]
        public void SetUp()
        {
            _node1To1 = new NCoverNode(1, 0, 1, 1, "doc1", 0, false, "m", 0, "k", new Regex(".*"));
            _node2To2 = new NCoverNode(2, 0, 2, 1, "doc1", 0, false, "m", 0, "k", new Regex(".*"));
            _node4To4 = new NCoverNode(4, 0, 4, 1, "doc1", 0, false, "m", 0, "k", new Regex(".*"));
            _node5To5 = new NCoverNode(5, 0, 5, 1, "doc1", 0, false, "m", 0, "k", new Regex(".*"));

            _resultsNone = new NCoverResults(new NCoverNode[] {});
            _resultsAll = new NCoverResults(new[] {_node1To1, _node2To2, _node4To4, _node5To5});
            _results1To1N4To4 = new NCoverResults(new[] {_node1To1, _node4To4});
        }

        private NCoverNode _node1To1;
        private NCoverNode _node2To2;
        private NCoverNode _node4To4;
        private NCoverNode _node5To5;
        private NCoverResults _resultsAll;
        private NCoverResults _resultsNone;
        private NCoverResults _results1To1N4To4;

        [Test]
        public void ReportNewUntestedCode_DisplaysUntestedLines()
        {
            Assert.AreEqual("k m \t\t Line 2-2 in doc1\nk m \t\t Line 5-5 in doc1\n",
                _resultsAll.ReportNewUntestedCode(_results1To1N4To4));
            Assert.AreEqual("", _results1To1N4To4.ReportNewUntestedCode(_resultsAll));
        }

        [Test]
        public void ReportNewUntestedCode_JoinsBlocksOfuntestedCodeIntoASingleEntry()
        {
            Assert.AreEqual("k m \t\t Line 1-2 in doc1\nk m \t\t Line 4-5 in doc1\n",
                _resultsAll.ReportNewUntestedCode(_resultsNone));
        }
    }
}