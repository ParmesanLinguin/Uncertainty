namespace Uncertainty.Parsing.AST.Nodes;

public record class VariableDeclarationNode : ExpressionNode
{
    public TokenNode Keyword { get; init; }

    public TokenNode Name { get; init; }

    public TokenNode? Colon { get; init; }

    public TypeNode? Type { get; init; }

    public TokenNode Equal { get; init; }

    public ExpressionNode Expression { get; init; }

    public VariableDeclarationNode(TokenNode keyword, TokenNode name, TokenNode? colon,
        TypeNode? type, TokenNode equal, ExpressionNode expression)
    {
        Keyword = keyword;
        Name = name;
        Colon = colon;
        Type = type;
        Equal = equal;
        Expression = expression;
    }
}
