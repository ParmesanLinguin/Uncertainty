namespace Uncertainty.Parsing.AST.Nodes;

public record class ParenExpressionNode : AtomNode
{
    public TokenNode LParen { get; init; }

    public ExpressionNode Expression { get; init; }

    public TokenNode RParen { get; init; }

    public ParenExpressionNode(TokenNode lparen, ExpressionNode expression, TokenNode rparen)
    {
        LParen = lparen;
        Expression = expression;
        RParen = rparen;
    }
}
