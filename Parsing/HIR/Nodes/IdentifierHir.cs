using Uncertainty.Tokenization;

namespace Uncertainty.Parsing.HIR.Nodes;

internal class IdentifierHir : HirNode
{
    public string Value { get; init; }

    public Token Identifier { get; init; }

    public IdentifierHir(Token identifier)
    {
        Value = identifier.Content ?? throw new Exception("Identifier had no content!");
        Identifier = identifier;
    }
}
