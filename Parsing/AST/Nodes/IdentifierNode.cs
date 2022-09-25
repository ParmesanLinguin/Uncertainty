namespace Uncertainty.Parsing.AST.Nodes;

public record class IdentifierNode : AtomNode
{
    public TokenNode Identifier { get; init; }

    public IdentifierNode(TokenNode ident)
    {
        Identifier = ident;
    }
}
