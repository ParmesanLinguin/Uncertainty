namespace Uncertainty.Parsing.AST.Nodes;

public record class UnitLiteralNode : AtomNode
{
    public TokenNode Literal { get; init; }

    public UnitLiteralNode(TokenNode literal)
    {
        Literal = literal;
    }
}
