namespace Uncertainty.Parsing.HIR.Nodes;
record class FunctionDeclarationHir : HirNode
{
    public IdentifierHir Name { get; init; }

    public ParameterHir[] Parameters { get; init; }

    public TypeParameterHir[] TypeParameters { get; init; }

    public TypeHir ReturnType { get; init; }

    public FunctionDeclarationHir(IdentifierHir name, ParameterHir[] parameters, TypeParameterHir[] typeParameters, TypeHir returnType)
    {
        Name = name;
        Parameters = parameters;
        TypeParameters = typeParameters;
        ReturnType = returnType;
    }
}
