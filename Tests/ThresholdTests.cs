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
            _uncovered5 = new NCoverNode(5, 0, 5, 1, "doc", 0, false, "m", 0, "k", new Regex(".*"));
            _covered1 = new NCoverNode(1, 0, 1, 1, "doc", 1, false, "m", 0, "k", new Regex(".*"));
            _covered2 = new NCoverNode(2, 0, 2, 1, "doc", 1, false, "m", 0, "k", new Regex(".*"));
            _covered3 = new NCoverNode(3, 0, 3, 1, "doc", 1, false, "m", 0, "k", new Regex(".*"));
            _covered4 = new NCoverNode(4, 0, 4, 1, "doc", 1, false, "m", 0, "k", new Regex(".*"));
            _covered5 = new NCoverNode(5, 0, 5, 1, "doc", 1, false, "m", 0, "k", new Regex(".*"));

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
        private NCoverNode _uncovered5;

        private NCoverNode _covered1;
        private NCoverNode _covered2;
        private NCoverNode _covered3;
        private NCoverNode _covered4;
        private NCoverNode _covered5;

        [Test]
        public void Message_Fail_WhenPercentageDropsAndUncoveredCodeGrows()
        {
            var threshold = new Threshold(_results66Percent, _results50PercentOf4Lines, 0.30);
            Assert.AreEqual(
                "NCoverCopTask: FAILED: -- WARNING: ** Uncovered code introduced! **\r\n\tSequence Points Summary: 4 not excluded, 2 hit\r\n\tPercentage Summary: 50.00% [Minimum Required: 66.67%]\r\nk m \t\t Line 2-2 in doc\r\n",
                threshold.Message);
        }

        [Test]
        public void Message_Pass_WhenNewPercentageBeatsMinThreshold()
        {
            var threshold = new Threshold(_results50PercentOf4Lines, _results100Percent, 0.60);
            Assert.AreEqual("NCoverCopTask: PASSED: -- There is no newly uncovered code. \r\n\tSequence Points Summary: 3 not excluded, 3 hit\r\n\tPercentage Summary: 100.00% [Minimum Required: 60.00%]\r\n", threshold.Message);
        }

        [Test]
        public void Message_Pass_WhenNewPercentageBeatsMinThresholdAndPreviousPercentage()
        {
            var threshold = new Threshold(_results50PercentOf4Lines, _results100Percent, 0.30);
            Assert.AreEqual("NCoverCopTask: PASSED: -- There is no newly uncovered code. \r\n\tSequence Points Summary: 3 not excluded, 3 hit\r\n\tPercentage Summary: 100.00% [Minimum Required: 50.00%]\r\n", threshold.Message);
        }

        [Test]
        public void Message_Pass_WhenPercentageDropsButUncoveredCodeDoesNotGrow()
        {
            var threshold = new Threshold(_results66Percent, _results50PercentOf2Lines, 0.30);
            Assert.AreEqual(
                "NCoverCopTask: PASSED: -- There is no newly uncovered code. \r\n\tSequence Points Summary: 2 not excluded, 1 hit\r\n\tPercentage Summary: 50.00% [Minimum Required: 66.67%]\r\n",
                threshold.Message);
        }


        [Test]
        public void Message_PassWithDiff_WhenPercentageIncreasesButNewUncoveredCodeIntroduced()
        {
            var before = new NCoverResults(new[] { _covered1, _uncovered2, _uncovered3 });
            var after  = new NCoverResults(new[] { _covered1, _uncovered2, _covered3, _uncovered4, _uncovered5 });

            var threshold = new Threshold(before, after, 0.30);
            Assert.AreEqual(
                "NCoverCopTask: PASSED: -- WARNING: ** Uncovered code introduced! ** -- but your coverage is better, so I'll let you off.\r\n\tSequence Points Summary: 5 not excluded, 2 hit\r\n\tPercentage Summary: 40.00% [Minimum Required: 33.33%]\r\nk m \t\t Line 4-5 in doc\r\n",
                threshold.Message);
        }

        [Test]
        public void Should_Consolidate_Multiple_References_To_The_Same_codepoint_from_multiple_files()
        {
            var c1 = new NCoverNode(1, 0, 1, 1, "doc", 0, false, "m", 0, "k", new Regex(".*"));
            var c2 = new NCoverNode(1, 0, 1, 1, "doc", 1, false, "m", 0, "k", new Regex(".*"));

            Threshold threshold = new Threshold(
                new NCoverResults(new INCoverNode[] {c1, c2}),
                new NCoverResults(new INCoverNode[] {c2}),
                0.0);

            Assert.AreEqual(
                "NCoverCopTask: PASSED: -- There is no newly uncovered code. \r\n\tSequence Points Summary: 1 not excluded, 1 hit\r\n\tPercentage Summary: 100.00% [Minimum Required: 100.00%]\r\n",
                threshold.Message);
        }

    }
}