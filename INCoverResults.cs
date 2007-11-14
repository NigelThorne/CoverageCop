namespace NCoverCop
{
    public interface INCoverResults
    {
        double PercentageCovered { get; }

        double Total { get; }

        double TotalVisited { get; }

        double TotalUnvisited { get; }

        string ReportNewUntestedCode(INCoverResults previous);
        bool HasMatchingUnvisitedNode(INCoverNode node);
    }
}