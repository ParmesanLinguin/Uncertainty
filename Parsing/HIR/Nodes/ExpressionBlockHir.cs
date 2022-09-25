namespace Uncertainty.Parsing.HIR.Nodes;

record class ExpressionBlockHir : HirNode, HirElseCase
{
    public HirExpression[] Expressions { get; init; }

    public ExpressionBlockHir(HirExpression[] expressions)
    {
        Expressions = expressions;
    }
}
