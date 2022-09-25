namespace Uncertainty.Parsing.AST.Nodes;

public record class ArgListNode : AstNode
{
    public TokenNode LParen { get; init; }

    public ExpressionNode[] Params { get; init; }
    
    public TokenNode[] Commas { get; init; }

    public TokenNode RParen { get; init; }

    public ArgListNode(TokenNode lparen, ExpressionNode[] arguments, TokenNode[] commas, TokenNode rparen)
    {
        LParen = lparen;
        Params = arguments;
        Commas = commas;
        RParen = rparen;
    }
}
