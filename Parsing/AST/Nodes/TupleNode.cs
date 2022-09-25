namespace Uncertainty.Parsing.AST.Nodes;

public record class TupleNode : AstNode
{
    public TokenNode LParen { get; init; }

    public ExpressionNode[] Elements { get; init; }

    public TokenNode[] Commas { get; init; }

    public TokenNode RParen { get; init; }

    public TupleNode(TokenNode lparen, ExpressionNode[] elements, TokenNode[] commas, TokenNode rparen)
    {
        LParen = lparen;
        Elements = elements;
        Commas = commas;
        RParen = rparen;
    }
}
