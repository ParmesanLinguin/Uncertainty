namespace Uncertainty.Parsing.AST.Nodes;

public record class PathExpressionNode : ExpressionNode
{
    public TokenNode? DoubleColon { get; init; }

    public PathSegmentNode[] Segments { get; init; }

    public ExpressionNode Expression { get; init; }

    public PathExpressionNode(TokenNode? doubleColon, PathSegmentNode[] segments, ExpressionNode expression)
    {
        DoubleColon = doubleColon;
        Segments = segments;
        Expression = expression;
    }
}
