using Uncertainty.Parsing.Types;

namespace Uncertainty.Parsing.HIR.Nodes;

record class TypeHir : HirNode
{
    public IType Type { get; init; }

    public string Name => Type.Name;

    public TypeHir(IType type)
    {
        Type = type;
    }

    public override string ToString()
    {
        return $"TypeHir {{ {Type.QualifiedName} }}";
    }
}