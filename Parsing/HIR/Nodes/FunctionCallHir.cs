using Uncertainty.Parsing.Types;

namespace Uncertainty.Parsing.HIR.Nodes;

record class FunctionCallHir : HirNode
{
    public IFunction Function { get; init; }

    public HirExpression[] Arguments { get; init; }

    public FunctionCallHir(IFunction function, HirExpression[] arguments)
    {
        Function = function;
        Arguments = arguments;
    }
}
