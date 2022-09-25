namespace Uncertainty.Parsing.Types;

internal interface IFunction : IModuleElement
{
    IType ReturnType { get; init; }

    IType[] Parameters { get; init; }

    TypeParameter[] TypeParameters { get; init; }
}
