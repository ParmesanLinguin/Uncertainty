namespace Uncertainty.Parsing.Types;

record class TypeParameter
{
    string Name { get; init; }

    public TypeParameter(string name)
    {
        Name = name;
    }
}
