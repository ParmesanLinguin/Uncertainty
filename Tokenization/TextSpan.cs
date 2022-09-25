namespace Uncertainty.Tokenization;

public readonly record struct TextSpan
{
    public readonly LineCol Start { get; init; }

    public readonly LineCol End { get; init; }

    public TextSpan(LineCol start, LineCol end)
    {
        Start = start;
        End = end;
    } 

    public override string ToString()
    {
        return $"({Start}, {End})";
    }
}