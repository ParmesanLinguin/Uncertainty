namespace Uncertainty.Parsing.AST.Nodes;

public record class StatementNode : AstNode
{
    public ExpressionNode Expression { get; init; }

    public TokenNode Newline { get; init; }

    public StatementNode(ExpressionNode expression, TokenNode newline)
    {
        Expression = expression;
        Newline = newline;
    }
}
