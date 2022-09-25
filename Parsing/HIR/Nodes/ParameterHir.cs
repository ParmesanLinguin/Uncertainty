namespace Uncertainty.Parsing.HIR.Nodes;
record class ParameterHir : HirNode
{
    public IdentifierHir Name { get; init; }

    public TypeHir Type { get; init; }

    public ParameterHir(IdentifierHir name, TypeHir type)
    {
        Name = name;
        Type = type;
    }
}
