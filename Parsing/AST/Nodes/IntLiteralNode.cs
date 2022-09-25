namespace Uncertainty.Parsing.AST.Nodes;

public record class IntLiteralNode : AtomNode
{
    public TokenNode Int { get; init; }

    public IntLiteralNode(TokenNode num)
    {
        Int = num;
    }
}
