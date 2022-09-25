namespace Uncertainty.Tokenization;

public record class Token
{
    public TextSpan? Position { get; init; }
    public TokenType Type { get; init; }
    public string? Content { get; init; }

    public Token(TokenType type, string? content, TextSpan? position)
    {
        Type = type;
        Content = content;
        Position = position;
    }

    public override string ToString()
    {
        return $"Token {Type} {{ Content: '{Content}', Position: {Position} }}"
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
    }
}

public enum TokenType
{
    // Symbols
    LParen,
    RParen,
    Colon,
    Quote,
    Percent,
    Comma,
    Dollar,
    Underscore,
    Period,
    DoubleColon,
    Plus,
    Minus,
    Equals,
    DoubleEquals,
    Gt,
    Lt,
    LBrace,
    RBrace,
    Asterisk,
    Slash,
    TypeArrow,
    
    // Indentation
    IND_Indent,
    IND_Outdent,

    // Keywords
    KW_If,
    KW_Else,
    KW_Break,
    KW_Continue,
    KW_While,
    KW_Def,
    KW_Let,
    KW_Var,

    // Other
    IntLiteral,
    StringLiteral,
    UnitLiteral,
    Identifier,

    Whitespace,
    Newline,
    EOF
}
