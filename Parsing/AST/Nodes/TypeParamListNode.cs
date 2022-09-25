namespace Uncertainty.Parsing.AST.Nodes;

public record class TypeParamListNode : AstNode
{
    public TokenNode Lt { get; init; }

    public TypeNode[] Params { get; init; }

    public TokenNode[] Commas { get; init; }

    public TokenNode Gt { get; init; }

    public TypeParamListNode(TokenNode lt, TypeNode[] paramList, TokenNode[] commas, TokenNode gt)
    {
        Lt = lt;
        Params = paramList;
        Commas = commas;
        Gt = gt;
    }
}
