using System.Text.RegularExpressions;
using NUnit.Framework;


namespace NCoverCop.Tests
{
    [TestFixture]
    public class NCoverNodeTests 
    {
        private NCoverNode node1to1;
        private NCoverNode node1to5;
        private NCoverNode node5to10;
        private NCoverNode node6to10;

        [SetUp]
        public  void SetUp()
        {
            node1to1 = new NCoverNode(1, 0, 1, 1, "doc1", 0, false, new Regex(".*"));
            node1to5 = new NCoverNode(1, 0, 5, 1, "doc1", 0, false, new Regex(".*"));
            node5to10 = new NCoverNode(5, 0, 10, 1, "doc1", 0, false, new Regex(".*"));
            node6to10 = new NCoverNode(6, 0, 10, 1, "doc1", 0, false, new Regex(".*"));

        }

        [Test]
        public void Follows_IsTrue_WhenDocumentIsTheSameAndStartLineOfSecondNodeEqualsEndLineOfTheOtherNode()
        {
            Assert.IsTrue(node5to10.Follows(node1to5));            
        }

        [Test]
        public void Follows_IsTrue_WhenDocumentIsTheSameAndStartLineOfSecondNodeFollowsTheEndLineOfTheOtherNode()
        {
            Assert.IsFalse(node1to5.Follows(node6to10));
            Assert.IsTrue(node6to10.Follows(node1to5));
        }

        [Test]
        public void ExtendWith_SameNode_WhenTwoMatchingNodesAreUsed()
        {
            INCoverNode result = node1to1.ExtendWith(node1to1);
            Assert.AreEqual(1, result.Line);
            Assert.AreEqual(1, result.EndLine);
            Assert.AreEqual(node1to1.Document, result.Document);
        }

        [Test]
        public void ExtendWith_ExtendedNode_WhenTwoFollowingNodesAreUsed()
        {
            INCoverNode result = node1to5.ExtendWith(node6to10);
            Assert.AreEqual(1, result.Line);
            Assert.AreEqual(10, result.EndLine);
            Assert.AreEqual(node1to1.Document, result.Document);
        }

        [Test]
        public void Matches_True_SameNode()
        {
            Assert.IsTrue(node1to5.Matches(node1to5));
        }

        [Test]
        public void Document_IsTruncatedByMatchingRegEx()
        {
            NCoverNode node = new NCoverNode(1, 2, 3, 4, @"C:/Manticore_Debug/trunk/SomeFile.cs", 1, false, new Regex("trunk.*"));
            Assert.AreEqual(@"trunk/SomeFile.cs", node.Document);
        }

        [Test]
        public void Document_IsTakesTheBiggestMatchingRegEx()
        {
            NCoverNode node = new NCoverNode(1, 2, 3, 4, @"C:/Manticore_Debug/trunk/trunk/trunkSomeFile.cs", 1, false, new Regex("trunk.*"));
            Assert.AreEqual(@"trunk/trunk/trunkSomeFile.cs", node.Document);
        }
    }
}