namespace Uncertainty.Tokenization;

readonly struct TokenizerPair
{
    readonly int Pos { get; init; }
    readonly char Char { get; init; }

    public TokenizerPair(char ch, int pos)
    {
        Char = ch;
        Pos = pos;
    }
}