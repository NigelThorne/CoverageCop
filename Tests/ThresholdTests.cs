using NUnit.Framework;

namespace NCoverCop.Tests
{
    [TestFixture]
    public class ThresholdTests
    {
        private NCoverResults results100percent;
        private NCoverResults results50percentOf4Lines;
        private NCoverResults results50percentOf2Lines;
        private NCoverResults results66percent;

        private NCoverNode uncovered1;
        private NCoverNode uncovered2;
        private NCoverNode uncovered3;
        private NCoverNode uncovered4;

        private NCoverNode covered1;
        private NCoverNode covered2;
        private NCoverNode covered3;
        private NCoverNode covered4;

        [SetUp]
        public void SetUp()
        {
            uncovered1 = new NCoverNode(1, 0, 1, 1, "doc", false, false);
            uncovered2 = new NCoverNode(2, 0, 2, 1, "doc", false, false);
            uncovered3 = new NCoverNode(3, 0, 3, 1, "doc", false, false);
            uncovered4 = new NCoverNode(4, 0, 4, 1, "doc", false, false);
            covered1 = new NCoverNode(1, 0, 1, 1, "doc", true, false);
            covered2 = new NCoverNode(2, 0, 2, 1, "doc", true, false);
            covered3 = new NCoverNode(3, 0, 3, 1, "doc", true, false);
            covered4 = new NCoverNode(4, 0, 4, 1, "doc", true, false);

            results100percent = new NCoverResults(new NCoverNode[] {covered1, covered2, covered3});
            results66percent = new NCoverResults(new NCoverNode[] {uncovered1, covered2, covered3});
            results50percentOf4Lines = new NCoverResults(new NCoverNode[] {uncovered1, uncovered2, covered3, covered4});
            results50percentOf2Lines = new NCoverResults(new NCoverNode[] {uncovered1, covered2});
        }

        [Test]
        public void Message_Pass_WhenNewPercentageBeatsMinThreshold()
        {
            Threshold threshold = new Threshold(results50percentOf4Lines, results100percent, 0.60);
            Assert.AreEqual("NCoverCopTask: PASSED: 3 not excluded, 3 hit, 100.00 % >= 60.00 %", threshold.Message);
        }

        [Test]
        public void Message_Pass_WhenNewPercentageBeatsMinThresholdAndPreviousPercentage()
        {
            Threshold threshold = new Threshold(results50percentOf4Lines, results100percent, 0.30);
            Assert.AreEqual("NCoverCopTask: PASSED: 3 not excluded, 3 hit, 100.00 % >= 50.00 %", threshold.Message);
        }

        [Test]
        public void Message_Pass_WhenPercentageDropsButUncoveredCodeDoesNotGrow()
        {
            Threshold threshold = new Threshold(results66percent, results50percentOf2Lines, 0.30);
            Assert.AreEqual("NCoverCopTask: PASSED: 2 not excluded, 1 hit, 50.00 % >= 66.67 % but uncovered code has not grown.", threshold.Message);
        }

        [Test]
        public void Message_Fail_WhenPercentageDropsAndUncoveredCodeGrows()
        {
            Threshold threshold = new Threshold(results66percent, results50percentOf4Lines, 0.30);
            Assert.AreEqual("NCoverCopTask: FAILED: 4 not excluded, 2 hit, 50.00 % < 66.67 %\nLine 2-2 in doc\n", threshold.Message);
        }
    }
}