using NUnit.Framework;


namespace NCoverCop.Tests
{
    [TestFixture]
    public class NCoverNodeTests : MockingTestFixture
    {
        private NCoverNode nCoverNode;

        protected override void SetUp()
        {
            nCoverNode = new NCoverNode(1,1,2,2,"doc1");

        }

        [Test]
        public void Follows_IsTrue_WhenDocumentIsTheSameAndStartLineOfSecondNodeEqualsEndLineOfTheOtherNode()
        {
            NCoverNode nCoverNode1 = new NCoverNode(1, 1, 1, 1, "doc1");
            NCoverNode nCoverNode2 = new NCoverNode(1, 1, 1, 1, "doc1");
            Assert.IsTrue(nCoverNode2.Follows(nCoverNode1));            
        }

        [Test]
        public void Follows_IsTrue_WhenDocumentIsTheSameAndStartLineOfSecondNodeFollowsTheEndLineOfTheOtherNode()
        {
            NCoverNode nCoverNode1 = new NCoverNode(1, 0, 3, 0, "doc1");
            NCoverNode nCoverNode2 = new NCoverNode(4, 0, 6, 0, "doc1");
            Assert.IsTrue(nCoverNode2.Follows(nCoverNode1));
        }
    }
}