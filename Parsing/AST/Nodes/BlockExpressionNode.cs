namespace Uncertainty.Parsing.AST.Nodes;

public record class BlockExpressionNode : ElseExpression, AstNode
{
    public TokenNode Indent { get; init; }

    public StatementNode[] Statements { get; init; }

    public ExpressionNode Expression { get; init; }

    public TokenNode Outdent { get; init; }

    public BlockExpressionNode(TokenNode indent, StatementNode[] statements, ExpressionNode expression, TokenNode outdent)
    {
        Indent = indent;
        Statements = statements;
        Expression = expression;
        Outdent = outdent;
    }
}
