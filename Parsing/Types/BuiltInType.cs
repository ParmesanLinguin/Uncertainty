using Uncertainty.Parsing.Types;

namespace Uncertainty.Parsing.Types;

internal class BuiltInType : IType
{
    public string Name { get; init; }

    public IModule? ParentModule { get; init; }

    public string[] GenericArguments { get; init; }

    public BuiltInType(string name, IModule parentModule, string[] genericArguments)
    {
        Name = name;
        ParentModule = parentModule;
        GenericArguments = genericArguments;
    }
}
