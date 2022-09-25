namespace Uncertainty.Parsing.AST.Nodes;

public record class ParamListNode : AstNode
{
    public TokenNode LParen { get; init; }

    public TokenNode[] ParamsAndCommas { get; init; }

    public TypeNode[] ParamTypes { get; init; }

    public TokenNode RParen { get; init; }

    public ParamListNode(TokenNode lparen, TokenNode[] paramList, TypeNode[] paramTypes, TokenNode rparen)
    {
        LParen = lparen;
        ParamsAndCommas = paramList;
        ParamTypes = paramTypes;
        RParen = rparen;
    }
}
