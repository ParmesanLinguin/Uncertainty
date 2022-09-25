namespace Uncertainty.Parsing.HIR.Nodes;

record class TypeParameterHir : HirNode
{
    public string Name { get; init; }

    public TypeParameterHir(string name)
    {
        Name = name;
    }
}