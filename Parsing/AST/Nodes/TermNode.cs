namespace Uncertainty.Parsing.AST.Nodes;

public record class TermNode : ExpressionNode
{
    public ItemNode Left { get; init; }

    public ExpressionNode[] Items { get; init; }

    public TokenNode[] Operators { get; init; }
    
    public TermNode(ItemNode left, ExpressionNode[] items, TokenNode[] operators)
    {
        Left = left;
        Items = items;
        Operators = operators;
    }
}
