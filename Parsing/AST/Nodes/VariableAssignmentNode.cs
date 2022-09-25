namespace Uncertainty.Parsing.AST.Nodes;

public record class VariableAssignmentNode : ExpressionNode
{
    public TokenNode Name { get; init; }

    public TokenNode Equal { get; init; }

    public ExpressionNode Expression { get; init; }

    public VariableAssignmentNode(TokenNode name, TokenNode equal, ExpressionNode expression)
    {
        Name = name;
        Equal = equal;
        Expression = expression;
    }
}
