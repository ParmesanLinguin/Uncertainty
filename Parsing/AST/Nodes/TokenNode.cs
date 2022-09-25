using Uncertainty.Tokenization;

namespace Uncertainty.Parsing.AST.Nodes;

public record class TokenNode : AstNode
{
    public Token Token { get; init; }

    public TokenNode(Token token)
    {
        Token = token;
    }

    public override string ToString()
    {
        return $"Token ({Token})";
    }
}
