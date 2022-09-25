using Uncertainty.Parsing.HIR.Nodes;
using Uncertainty.Parsing.AST.Nodes;
using Uncertainty.Parsing.Types;

namespace Uncertainty.Parsing.HIR;

class AstToHir
{
    IModule currentModule;
    GlobalScope elements;
    ProgramNode node;

    public AstToHir(ProgramNode root)
    {
        node = root;
        var builtins = new Builtins().Module;
        elements = new GlobalScope();
        elements.RegisterModule(builtins);
        currentModule = new BuiltInModule(new(), "code", null);
    }

    public ProgramHir ConvertProgram()
    {
        List<FunctionDeclarationHir> functions = new();
        foreach (var v in node.Statements)
        {
            if (v is FunctionDeclNode func)
            {
                functions.Add(ConvertFunctionDeclaration(func));
            }
        }
        ModuleHir module = new ModuleHir(functions.ToArray(), currentModule);
        return new ProgramHir(new[] { module });
    }

    void RegisterModuleElements()
    {
        // Types
        // Traits
        // Functions
        foreach (var v in node.Statements)
        {
            if (v is FunctionDeclNode func)
            {
                var retType = FindType(func.ReturnType.Name)
                currentModule.ModuleElements.Add(func.Name.Token.Content!);
            }
        }
    }

    FunctionDeclarationHir ConvertFunctionDeclaration(FunctionDeclNode node)
    {
        IdentifierHir name = ConvertIdentifier(node.Name);
        List<ParameterHir> parameters = new();
        List<TypeParameterHir> typeParams = new();
        TypeHir returnType = ConvertType(node.ReturnType);

        for (int i = 0; i < node.ParamList.ParamTypes.Length; i++)
        {
            var n = ConvertIdentifier(node.ParamList.ParamsAndCommas[i * 2]);
            var t = ConvertType(node.ParamList.ParamTypes[i]);
            parameters.Add(new ParameterHir(n, t));
        }

        if (node.TypeList is not null)
        {
            foreach (var t in node.TypeList.Params)
            {
                typeParams.Add(new (t.Name.Token.Content!));
            }
        }

        return new FunctionDeclarationHir(
            name,
            parameters.ToArray(),
            typeParams.ToArray(),
            returnType
        );
    }

    IdentifierHir ConvertIdentifier(TokenNode ident)
    {
        if (ident.Token.Type != Tokenization.TokenType.Identifier)
        {
            throw new ArgumentException("Not an indentifier", nameof(ident));
        }
        return new IdentifierHir(ident.Token);
    }

    TypeHir ConvertType(TypeNode typeNode)
    {
        string name = typeNode.Name.Token.Content ?? throw new Exception("Type name is empty!");
        return new TypeHir(FindType(name));
    }

    ExpressionBlockHir ConvertBlockExpression(BlockExpressionNode b)
    {
        return new(Array.Empty<HirExpression>());
        
    }

    HirExpression ConvertExpression(ExpressionNode e)
    {
        return e switch
        {
            FunctionCallNode n => ConvertFunctionCall(n),
            IfExpressionNode n => ConvertIfExpression(n),
            PathExpressionNode n => ConvertPathExpression(n),
            TermNode n => ParseTerm(n),
            VariableAssignmentNode n => ParseVariableAssignment(n),
            VariableDeclarationNode n => ParseVariableDeclaration(n),
            WhileExpressionNode n => ParseWhileExpression(n),
            _ => throw new NotImplementedException("Not implemented for type " + e.GetType().Name),
        };
    }

    FunctionCallHir ConvertFunctionCall(FunctionCallNode n)
    {
        IFunction fn = FindFunction(n.Function.Token.Content!);
        List<HirExpression> expressions = new();
        foreach (var e in n.ArgList.Params)
        {
            expressions.Add(ConvertExpression(e));
        }
        return new FunctionCallHir(fn, expressions.ToArray());
    }

    IfExpressionHir ConvertIfExpression(IfExpressionNode n)
    {
        var expr = ConvertExpression(n.Expression);
        var ifBlock = ConvertBlockExpression(n.Block);
        HirElseCase? elseCase = n.ElseBlock is null ? null : ConvertElseCase(n.ElseBlock);

        return new IfExpressionHir(expr, ifBlock, elseCase);
    }

    //HirExpression ConvertPathExpression()
    //{

    //}

    HirElseCase ConvertElseCase(ElseExpression expr)
    {
        return expr switch
        {
            IfExpressionNode ifNode => ConvertIfExpression(ifNode),
            BlockExpressionNode block => ConvertBlockExpression(block),
        };
    }

    IFunction FindFunction(string name, IModule? module = null)
    {
        IModuleElement val;
        if (module is not null)
        {
            if (!module.ModuleElements.TryGetValue(name, out val!)) { throw new Exception($"No functions found by name '{name}'"); }
        } 
        else
        {
            if (!elements.TryGetValue(name, out val!)) { throw new Exception($"No functions found by name '{name}'"); }
        }
        
        if (val is not IFunction fn)
        {
            throw new Exception($"Cannot call {val.QualifiedName} as a function (it is {val.GetType()})");
        }
        return fn;
    }

    IType FindType(string name, IModule? module = null)
    {
        IModuleElement val;
        if (module is not null)
        {
            if (!module.ModuleElements.TryGetValue(name, out val!)) { throw new Exception($"No types found by name '{name}'"); }
        }
        else
        {
            if (!elements.TryGetValue(name, out val!)) { throw new Exception($"No types found by name '{name}'"); }
        }

        if (val is not IType fn)
        {
            throw new Exception($"Cannot use {val.QualifiedName} as a type (it is {val.GetType()})");
        }
        return fn;
    }
}
