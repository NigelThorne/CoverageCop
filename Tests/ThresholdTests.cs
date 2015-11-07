using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NCoverCop.Tests
{
    [TestFixture]
    public class ThresholdTests
    {
        [SetUp]
        public void SetUp()
        {
            _uncovered1 = new NCoverNode(1, 0, 1, 1, "doc", 0, false, "m", 0, "k", new Regex(".*"));
            _uncovered2 = new NCoverNode(2, 0, 2, 1, "doc", 0, false, "m", 0, "k", new Regex(".*"));
            _uncovered3 = new NCoverNode(3, 0, 3, 1, "doc", 0, false, "m", 0, "k", new Regex(".*"));
            _uncovered4 = new NCoverNode(4, 0, 4, 1, "doc", 0, false, "m", 0, "k", new Regex(".*"));
            _covered1 = new NCoverNode(1, 0, 1, 1, "doc", 1, false, "m", 0, "k", new Regex(".*"));
            _covered2 = new NCoverNode(2, 0, 2, 1, "doc", 1, false, "m", 0, "k", new Regex(".*"));
            _covered3 = new NCoverNode(3, 0, 3, 1, "doc", 1, false, "m", 0, "k", new Regex(".*"));
            _covered4 = new NCoverNode(4, 0, 4, 1, "doc", 1, false, "m", 0, "k", new Regex(".*"));

            _results100Percent = new NCoverResults(new[] {_covered1, _covered2, _covered3});
            _results66Percent = new NCoverResults(new[] {_uncovered1, _covered2, _covered3});
            _results33Percent = new NCoverResults(new[] {_covered1, _uncovered2, _uncovered3});
            _results50PercentOf4Lines = new NCoverResults(new[] {_uncovered1, _uncovered2, _covered3, _covered4});
            _results50PercentOf2Lines = new NCoverResults(new[] {_uncovered1, _covered2});
        }

        private NCoverResults _results100Percent;
        private NCoverResults _results50PercentOf4Lines;
        private NCoverResults _results50PercentOf2Lines;
        private NCoverResults _results66Percent;
        private NCoverResults _results33Percent;

        private NCoverNode _uncovered1;
        private NCoverNode _uncovered2;
        private NCoverNode _uncovered3;
        private NCoverNode _uncovered4;

        private NCoverNode _covered1;
        private NCoverNode _covered2;
        private NCoverNode _covered3;
        private NCoverNode _covered4;

        [Test]
        public void Message_Fail_WhenPercentageDropsAndUncoveredCodeGrows()
        {
            var threshold = new Threshold(_results66Percent, _results50PercentOf4Lines, 0.30);
            Assert.AreEqual(
                "NCoverCopTask: FAILED: 4 not excluded, 2 hit, 50.00% < 66.67%\nk m \t\t Line 2-2 in doc\n",
                threshold.Message);
        }

        [Test]
        public void Message_Pass_WhenNewPercentageBeatsMinThreshold()
        {
            var threshold = new Threshold(_results50PercentOf4Lines, _results100Percent, 0.60);
            Assert.AreEqual("NCoverCopTask: PASSED: 3 not excluded, 3 hit, 100.00% >= 60.00%\n", threshold.Message);
        }

        [Test]
        public void Message_Pass_WhenNewPercentageBeatsMinThresholdAndPreviousPercentage()
        {
            var threshold = new Threshold(_results50PercentOf4Lines, _results100Percent, 0.30);
            Assert.AreEqual("NCoverCopTask: PASSED: 3 not excluded, 3 hit, 100.00% >= 50.00%\n", threshold.Message);
        }

        [Test]
        public void Message_Pass_WhenPercentageDropsButUncoveredCodeDoesNotGrow()
        {
            var threshold = new Threshold(_results66Percent, _results50PercentOf2Lines, 0.30);
            Assert.AreEqual(
                "NCoverCopTask: PASSED: 2 not excluded, 1 hit, 50.00% >= 66.67% but uncovered code has not grown.\n",
                threshold.Message);
        }


        [Test]
        public void Message_PassWithDiff_WhenPercentageIncreasesButNewUncoveredCodeIntroduced()
        {
            var uncoverLine4ButCoverLine3 = new NCoverResults(new[] {_covered1, _uncovered2, _covered3, _uncovered4});
            var threshold = new Threshold(_results33Percent, uncoverLine4ButCoverLine3, 0.30);
            Assert.AreEqual(
                "NCoverCopTask: PASSED: 4 not excluded, 2 hit, 50.00% >= 33.33%\nk m \t\t Line 4-4 in doc\n",
                threshold.Message);
        }
    }
}