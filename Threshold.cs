using System;

namespace NCoverCop
{
    public class Threshold
    {
        private readonly double minPercentage;
        private readonly NCoverResults previous;
        private readonly NCoverResults results;

        public Threshold(NCoverResults previous, NCoverResults results, double minPercentage)
        {
            this.results = results;
            this.minPercentage = minPercentage;
            this.previous = previous;
        }

        public bool Passed => results.PercentageCovered >= RequiredPercentage ||
                              results.TotalUnvisited <= RequiredUnvisitedTotal;

        private double RequiredUnvisitedTotal => previous?.TotalUnvisited ?? 0;

        public string Message
        {
            get
            {
                if (results.TotalUnvisited <= RequiredUnvisitedTotal)
                    return
    $@"NCoverCopTask: PASSED: -- There is no newly uncovered code. 
	Sequence Points Summary: {results.Total} not excluded, {results.TotalVisited} hit
	Percentage Summary: {results.PercentageCovered:p} [Minimum Required: {RequiredPercentage:p}]
{results.ReportNewUntestedCode(previous)}";

                if (results.PercentageCovered >= RequiredPercentage)
                    return
$@"NCoverCopTask: PASSED: -- WARNING: ** Uncovered code introduced! ** -- but your coverage is better, so I'll let you off.
	Sequence Points Summary: {results.Total} not excluded, {results.TotalVisited} hit
	Percentage Summary: {results.PercentageCovered:p} [Minimum Required: {RequiredPercentage:p}]
{results.ReportNewUntestedCode(previous)}";

                return
$@"NCoverCopTask: FAILED: -- WARNING: ** Uncovered code introduced! **
	Sequence Points Summary: {results.Total} not excluded, {results.TotalVisited} hit
	Percentage Summary: {results.PercentageCovered:p} [Minimum Required: {RequiredPercentage:p}]
{results.ReportNewUntestedCode(previous)}";

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