namespace Uncertainty.Parsing.AST.Nodes;

public record class FunctionDeclNode : TopLevelStatementNode
{
    public TokenNode Def { get; init; }
    
    public TokenNode Name { get; init; }

    public TypeParamListNode? TypeList { get; init; }

    public ParamListNode ParamList { get; init; }

    //public TokenNode TypeArrow { get; init; }

    public TypeNode ReturnType { get; init; }

    public TokenNode Colon { get; init; }

    public BlockExpressionNode Block { get; init; }

    public FunctionDeclNode(TokenNode def, TokenNode name, TypeParamListNode? typeList, 
        ParamListNode paramList, /*TokenNode typeArrow,*/ TypeNode returnType, TokenNode colon,
        BlockExpressionNode block)
    {
        Def = def;
        Name = name;
        TypeList = typeList;
        ParamList = paramList;
        //TypeArrow = typeArrow;
        ReturnType = returnType;
        Colon = colon;
        Block = block;
    }
}
