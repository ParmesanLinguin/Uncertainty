namespace Uncertainty.Parsing.AST.Nodes;

public record class WhileExpressionNode : ExpressionNode
{
    public TokenNode While { get; init; }

    public ExpressionNode Expression { get; init; }

    public TokenNode Colon { get; init; }

    public BlockExpressionNode Block { get; init; }

    public WhileExpressionNode(TokenNode kwwhile, ExpressionNode expression, TokenNode colon, BlockExpressionNode block)
    {
        While = kwwhile;
        Expression = expression;
        Colon = colon;
        Block = block;
    }
}
