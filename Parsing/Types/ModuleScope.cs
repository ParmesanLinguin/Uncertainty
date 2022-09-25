using System.Diagnostics.CodeAnalysis;

namespace Uncertainty.Parsing.Types;

internal class GlobalScope
{
    public Dictionary<string, List<IModuleElement>> Elements { get; set; }

    public GlobalScope()
    {
        Elements = new();
    }

    public IModuleElement this[string index]
    {
        get { return Elements[index].Count <= 1 ? Elements[index][0] : throw new Exception($"Cannot resolve between {Elements[index][0].QualifiedName ?? "null!"} and {Elements[index][1].QualifiedName ?? "null!"} (use a qualified name instead)"); }
        set { Elements[index].Add(value); }
    }

    public IModuleElement this[IModuleElement elem]
    {
        get { return this[elem.Name]; }
    }

    public void Add(IModuleElement element)
    {
        if (Elements.TryGetValue(element.Name, out var value))
        {
            value.Add(element);
        } else
        {
            Elements[element.Name] = new() { element };
        }
    }

    public bool TryGetValue(string name, [NotNullWhen(true)] out IModuleElement? result)
    {
        if (Elements.TryGetValue(name, out var value))
        {
            if (value.Count > 1)
            {
                throw new Exception($"Cannot resolve between {value[0].QualifiedName ?? "null!"} and {value[1].QualifiedName ?? "null!"} (use a qualified name instead)");
            } else
            {
                result = value[0];
                return true;
            }
        }
        else
        {
            result = null;
            return false;
        }
    }

    public bool RegisterModule(IModule module)
    {
        foreach (var elem in module.ModuleElements.Values)
        {
            Add(elem);
        }
        return true;
    }
}
