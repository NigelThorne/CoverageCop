using System;

namespace NCoverCop
{
    public class Threshold
    {
        private readonly NCoverResults results;
        private readonly double minPercentage;
        private readonly NCoverResults previous;

        public Threshold(NCoverResults previous, NCoverResults results, double minPercentage)
        {
            this.results = results;
            this.minPercentage = minPercentage;
            this.previous = previous;
        }

        public bool Passed
        {
            get
            {
                return results.PercentageCovered >= RequiredPercentage || results.TotalUnvisited <= RequiredUnvisitedTotal;
            }
        }

        private double RequiredUnvisitedTotal
        {
            get
            {
                if (previous == null) return 0;
                return previous.TotalUnvisited;
            }
        }


        public string Message
        {
            get
            {
                if (Passed)
                {
                    return
                        string.Format("NCoverCopTask: PASSED: {0} not excluded, {1} hit, {2:p} >= {3:p}",
                                      results.Total,
                                      results.TotalVisited, results.PercentageCovered, RequiredPercentage);
                }
                else
                {
                    return
                        string.Format("NCoverCopTask: FAILED: {0} not excluded, {1} hit, {2:p} < {3:p}\n{4}", results.Total,
                                      results.TotalVisited, results.PercentageCovered, RequiredPercentage, results.ReportNewUntestedCode(previous));
                }
            }
        }

        public double RequiredPercentage
        {
            get
            {
                double requiredPercentage = 0;
                if (previous != null) requiredPercentage = previous.PercentageCovered;
                return Math.Max(requiredPercentage, minPercentage) - 0.00001;
            }
        }
    }
}