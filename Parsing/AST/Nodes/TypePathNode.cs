namespace Uncertainty.Parsing.AST.Nodes;

public record class TypePathNode : AstNode
{
    public TokenNode? DoubleColon { get; init; }

    public PathSegmentNode[] Segments { get; init; }

    public TypePathNode(TokenNode? doubleColon, PathSegmentNode[] segments)
    {
        DoubleColon = doubleColon;
        Segments = segments;
    }
}
