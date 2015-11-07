using System;
using System.Collections.Generic;

namespace NCoverCop
{
    public class NCoverResults : INCoverResults
    {
        private readonly List<INCoverNode> unvisited = new List<INCoverNode>();

        public NCoverResults(IEnumerable<INCoverNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (!node.IsExcluded)
                {
                    Total++;

                    if (node.IsVisited)
                    {
                        TotalVisited++;
                    }
                    else
                    {
                        unvisited.Add(node);
                    }
                }
            }
        }

        #region INCoverResults Members

        public bool HasMatchingUnvisitedNode(INCoverNode node)
        {
            foreach (var unvisitedNode in unvisited)
            {
                if (unvisitedNode.Matches(node))
                {
                    return true;
                }
            }
            return false;
        }

        public double PercentageCovered
        {
            get { return Total == 0 ? 0 : Math.Round(TotalVisited/Total, 5); }
        }

        public string ReportNewUntestedCode(INCoverResults previous)
        {
            var nodes = new List<INCoverNode>();
            foreach (var node in unvisited)
            {
                if (!previous.HasMatchingUnvisitedNode(node))
                {
                    nodes.Add(node);
                }
            }

            INCoverNode lastNode = null;
            var condensed = new List<INCoverNode>();
            foreach (var node in nodes)
            {
                if (lastNode == null)
                {
                    lastNode = node;
                }
                else
                {
                    if (node.Follows(lastNode))
                    {
                        lastNode = lastNode.ExtendWith(node);
                    }
                    else
                    {
                        condensed.Add(lastNode);
                        lastNode = node;
                    }
                }
            }
            if (lastNode != null) condensed.Add(lastNode);

            var output = "";
            foreach (var node in condensed)
            {
                output += node + "\n";
            }
            return output;
        }

        public double Total { get; }

        public double TotalUnvisited
        {
            get { return Total - TotalVisited; }
        }

        public double TotalVisited { get; }

        #endregion
    }
}