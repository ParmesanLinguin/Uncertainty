namespace Uncertainty.Parsing.Types;

internal interface IType : IModuleElement
{
    string[] GenericArguments { get; init; }
}
