namespace Uncertainty.Parsing.AST.Nodes;

public record class PathSegmentNode : AstNode
{
    public TokenNode Identifier { get; init; }

    public TokenNode DoubleColon { get; init; }

    public PathSegmentNode(TokenNode identifier, TokenNode doubleColon)
    {
        Identifier = identifier;
        DoubleColon = doubleColon;
    }
}
