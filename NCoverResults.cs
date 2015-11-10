using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCoverCop
{
    public static class DictionaryExtensions
    {
        public static V Fetch<K,V>(this Dictionary<K,V> dict, K key, Func<V> defaultVal = null) where V : class, new()
        {
            V val;
            if(dict.TryGetValue(key, out val)) return val;
            val = defaultVal!=null ? defaultVal() : new V();
            dict[key] = val;
            return val;
        }
        public static V FF<K, V>(this Dictionary<K, V> dict, K key) where V : class
        {
            V val;
            return dict.TryGetValue(key, out val) ? val : null;
        }
    }

    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            var  ignore= list.All(item =>
            {
                action(item);
                return true;
            });
        }
    }

    public class NCoverResults : INCoverResults
    {
        private readonly Dictionary<string, Dictionary<string, List<INCoverNode>>> _unvisited;

        public NCoverResults(IEnumerable<INCoverNode> nodes)
        {
            _unvisited = new Dictionary<string, Dictionary<string, List<INCoverNode>>>();
            foreach (var node in nodes.Where(node => !node.IsExcluded))
            {
                var list = _unvisited.Fetch(node.Klass).Fetch(node.Method);
                var match = list.FirstOrDefault(n => node.Matches(n));
                if (match == null)
                {
                    list.Add(node);
                    Total++;
                    if (node.IsVisited)
                    {
                        TotalVisited++;
                    }
                }
                else
                {
                    if (!match.IsVisited && node.IsVisited)
                    {
                        list[list.IndexOf(match)] = node; // replace node. ? does this replace? 
                        TotalVisited++;
                    }
                }
            }

            // keep only unvisited.
            _unvisited.Values.ForEach(klass => 
                klass.Values.ForEach(method =>
                method.RemoveAll(node => node.IsVisited) 
                ));
        }

        #region INCoverResults Members

        public bool HasMatchingUnvisitedNode(INCoverNode node)
        {
            return _unvisited.FF(node.Klass)?.FF(node.Method)?.Any(unvisitedNode => unvisitedNode.Matches(node)) ?? false;
        }

        public double PercentageCovered => Total.IsZero() ? 100 : Math.Round(TotalVisited/Total, 5);

        public string ReportNewUntestedCode(INCoverResults previous)
        {
            var nodes = _unvisited.Values
                            .SelectMany(v => v.Values)
                            .SelectMany(v => v)
                            .Where(node => !previous.HasMatchingUnvisitedNode(node));

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
            return condensed.Aggregate(new StringBuilder(), (current, node) => current.AppendLine(node.ToString())).ToString();
        }

        public double Total { get; }

        public double TotalUnvisited => Total - TotalVisited;

        public double TotalVisited { get; }

        #endregion
    }
}