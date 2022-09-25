using Uncertainty.Parsing.Types;

namespace Uncertainty.Parsing.HIR.Nodes;

record class IfExpressionHir : HirExpression, HirElseCase
{
    public HirExpression Condition { get; init; }

    public ExpressionBlockHir IfCase { get; init; }

    public HirElseCase? ElseCase { get; init; }

    public IfExpressionHir(HirExpression condition, ExpressionBlockHir ifCase, HirElseCase? elseCase)
    {
        Condition = condition;
        IfCase = ifCase;
        ElseCase = elseCase;
    }
}
