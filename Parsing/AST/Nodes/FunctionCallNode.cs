namespace Uncertainty.Parsing.AST.Nodes;

public record class FunctionCallNode : ExpressionNode
{
    public TokenNode Function { get; init; }

    public ArgListNode ArgList { get; init; }

    public FunctionCallNode(TokenNode function, ArgListNode argList)
    {
        Function = function;
        ArgList = argList;
    }
}
