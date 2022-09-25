namespace Uncertainty.Parsing.AST.Nodes;

public record class IfExpressionNode : ExpressionNode, ElseExpression
{
    public TokenNode If { get; init; }

    public ExpressionNode Expression { get; init; }

    public TokenNode Colon { get; init; }

    public BlockExpressionNode Block { get; init; }

    public TokenNode? Else { get; init; }

    public ElseExpression? ElseBlock { get; init; }

    public TokenNode? ElseColon { get; init; }

    public IfExpressionNode(TokenNode kwif, ExpressionNode expression,
        TokenNode colon, BlockExpressionNode block, TokenNode? kwelse,
        ElseExpression? elseExpr, TokenNode? elseColon)
    {
        If = kwif;
        Expression = expression;
        Colon = colon;
        Block = block;
        Else = kwelse;
        ElseBlock = elseExpr;
        ElseColon = elseColon;
    }
}
