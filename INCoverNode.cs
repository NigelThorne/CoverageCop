namespace NCoverCop
{
    public interface INCoverNode
    {
        bool IsExcluded { get; }
        bool IsVisited { get; }

        int Line { get; }
        int Column { get; }
        int EndLine { get; }
        int EndColumn { get; }
        string Document { get; }

        bool Matches(INCoverNode ncoverNode);
        bool Follows(INCoverNode node);
        INCoverNode ExtendWith(INCoverNode node);
    }
}