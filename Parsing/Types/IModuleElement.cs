namespace Uncertainty.Parsing.Types;

interface IModuleElement
{
    public string Name { get; init; }

    public IModule? ParentModule { get; init; }

    public string QualifiedName => ParentModule is null ? $"::{Name}" : $"{ParentModule.QualifiedName}::{Name}";
}
