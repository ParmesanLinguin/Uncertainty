namespace Uncertainty.Tokenization;

public readonly record struct LineCol
{
    public readonly int Line { get; init; }

    public readonly int Col { get; init; }

    public LineCol(int line, int col)
    {
        Line = line;
        Col = col;
    }

    public override string ToString() => $"{Line}:{Col}";
}