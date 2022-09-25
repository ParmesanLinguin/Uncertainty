namespace Uncertainty.Parsing.AST.Nodes;

public record class TypeNode : AstNode
{
    public TypePathNode? Path { get; init; }

    public TokenNode Name { get; init; }

    public TypeParamListNode? TypeParamList { get; init; }

    public TypeNode(TypePathNode path, TokenNode name, TypeParamListNode? typeParamList)
    {
        Path = path;
        Name = name;
        TypeParamList = typeParamList;
    }
}
