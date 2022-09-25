namespace Uncertainty.Parsing.AST.Nodes;

public record class FactorNode : AstNode
{
    public AtomNode Left { get; init; }

    public TokenNode[] Ops { get; init; }

    public AtomNode[] Right { get; init; }

    public FactorNode(AtomNode left, TokenNode[] ops, AtomNode[] right)
    {
        Left = left;
        Ops = ops;
        Right = right;
    }
}
