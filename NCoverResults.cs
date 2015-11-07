using System;
using System.Collections.Generic;
using System.Linq;

namespace NCoverCop
{
    public class NCoverResults : INCoverResults
    {
        private readonly List<INCoverNode> _unvisited = new List<INCoverNode>();

        public NCoverResults(IEnumerable<INCoverNode> nodes)
        {
            foreach (var node in nodes.Where(node => !node.IsExcluded))
            {
                Total++;

                if (node.IsVisited)
                {
                    TotalVisited++;
                }
                else
                {
                    _unvisited.Add(node);
                }
            }
        }

        #region INCoverResults Members

        public bool HasMatchingUnvisitedNode(INCoverNode node)
        {
            return _unvisited.Any(unvisitedNode => unvisitedNode.Matches(node));
        }

        public double PercentageCovered => Total.IsZero() ? 100 : Math.Round(TotalVisited/Total, 5);

        public string ReportNewUntestedCode(INCoverResults previous)
        {
            var nodes = _unvisited.Where(node => !previous.HasMatchingUnvisitedNode(node)).ToList();

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

            return condensed.Aggregate("", (current, node) => current + (node + "\n"));
        }

        public double Total { get; }

        public double TotalUnvisited => Total - TotalVisited;

        public double TotalVisited { get; }

        #endregion
    }
}