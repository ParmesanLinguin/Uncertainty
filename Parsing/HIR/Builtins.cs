using Uncertainty.Parsing.Types;

namespace Uncertainty.Parsing.HIR;

internal class Builtins
{
    public BuiltInModule Module { get; init; }

    Dictionary<string, IModuleElement> elements;

    public Builtins()
    {
        elements = new();
        Module = new(elements, "builtin", null);
        CreateModuleElements();
    }

    private void CreateModuleElements()
    {
        Module.ModuleElements.Add("int", new BuiltInType("int", Module, new string[0]));
        Module.ModuleElements.Add("unit", new BuiltInType("unit", Module, new string[0]));

    }
}
