namespace Uncertainty.Parsing.AST.Nodes;

public record class ItemNode : AstNode
{
    public AtomNode Atom { get; init; }

    public ItemNode(AtomNode atom)
    {
        Atom = atom;
    }
}
