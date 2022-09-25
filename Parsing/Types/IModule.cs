namespace Uncertainty.Parsing.Types;

internal interface IModule : IModuleElement
{
    public Dictionary<string, IModuleElement> ModuleElements { get; init; }
}
