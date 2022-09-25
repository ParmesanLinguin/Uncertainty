namespace Uncertainty.Parsing.Types;

internal class BuiltInModule : IModule
{
    public Dictionary<string, IModuleElement> ModuleElements { get; init; }

    public string Name { get; init; }

    public IModule? ParentModule { get; init; }

    public BuiltInModule(Dictionary<string, IModuleElement> moduleElements, string name, IModule? parentModule)
    {
        ModuleElements = moduleElements;
        Name = name;
        ParentModule = parentModule;
    }
}
