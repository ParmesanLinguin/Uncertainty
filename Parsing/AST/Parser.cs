using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Uncertainty.Parsing.AST.Nodes;
using Uncertainty.Tokenization;

namespace Uncertainty.Parsing;

public class Parser
{
    IEnumerable<Token> tokensEnumerable;
    IEnumerator<Token> tokens;
    Token? Next;
    Token? Current;

    public Parser(IEnumerable<Token> tokens)
    {
        tokensEnumerable = tokens;
        this.tokens = tokensEnumerable.GetEnumerator();
        Advance();
        Advance();
    }

    public ProgramNode Parse()
    {
        return ParseProgram();
    }

    public ProgramNode ParseProgram()
    {
        TokenNode eof;
        List<TopLevelStatementNode> statements = new();
        while (!ConsumeInto(TokenType.EOF, out eof))
        {
            statements.Add(ParseTopLevelStatement());
            while (Consume(TokenType.Newline, out var _)) { }

        }
        return new ProgramNode(statements.ToArray(), eof);
    }

    public TopLevelStatementNode ParseTopLevelStatement()
    {
        if (PeekType(TokenType.KW_Def))
        {
            return ParseFunctionDeclaration();
        }

        Expect(TokenType.KW_Def);
        return null!;
    }

    public FunctionDeclNode ParseFunctionDeclaration()
    {
        TokenNode def = Expect(TokenType.KW_Def);
        TokenNode name = Expect(TokenType.Identifier);
        TypeParamListNode? typeParamList = null;
        if (PeekType(TokenType.LBrace))
        {
            typeParamList = ParseTypeParamList();
        }
        ParamListNode paramList = ParseParamList();
        //TokenNode typeArrow = Expect(TokenType.TypeArrow);
        TypeNode retType = ParseType();
        TokenNode colon = Expect(TokenType.Colon);
        BlockExpressionNode blockExpr = ParseBlockExpression();
        return new FunctionDeclNode(def, name, typeParamList, paramList, /*typeArrow,*/ retType, colon, blockExpr);
    }

    public TypeParamListNode ParseTypeParamList()
    {
        TokenNode lt = Expect(TokenType.LBrace);
        TokenNode gt;
        List<TypeNode> typeParams = new();
        List<TokenNode> commas = new List<TokenNode>();

        typeParams.Add(ParseType());
        while (ConsumeInto(TokenType.Comma, commas))
        {
            typeParams.Add(ParseType());
        }
        gt = Expect(TokenType.RBrace);

        return new TypeParamListNode(lt, typeParams.ToArray(), commas.ToArray(), gt);
    }

    public ParamListNode ParseParamList()
    {
        TokenNode lparen = Expect(TokenType.LParen);
        TokenNode rparen;
        List<TokenNode> paramNames = new();
        List<TypeNode> paramTypes = new();

        while (true)
        {
            if (!Consume(TokenType.RParen, out rparen!))
            {
                ExpectInto(TokenType.Identifier, paramNames);
                paramTypes.Add(ParseType());
                if (Consume(TokenType.RParen, out rparen!))
                {
                    break;
                }
                else
                {
                    ExpectInto(TokenType.Comma, paramNames);
                }
            } else { break; }
        }
        return new ParamListNode(lparen, paramNames.ToArray(), paramTypes.ToArray(), rparen);
    }

    public TypeNode ParseType()
    {
        TypePathNode? typePath = null;
        if (PeekType(TokenType.DoubleColon)) typePath = ParseTypePath();
        if (PeekType(TokenType.Identifier) && PeekType(TokenType.DoubleColon, skip: true))
        {
            typePath = ParseTypePath();
        }
        var name = Expect(TokenType.Identifier);
        TypeParamListNode? typeParamList = null;
        if (PeekType(TokenType.LBrace))
        {
            typeParamList = ParseTypeParamList();
        }

        return new TypeNode(typePath, name, typeParamList);
    }

    public TypePathNode ParseTypePath()
    {
        TokenNode? doubleColon = Consume(TokenType.DoubleColon);
        List<PathSegmentNode> segments = new List<PathSegmentNode>();
        while (PeekType(TokenType.Identifier) && PeekType(TokenType.DoubleColon, skip: true))
        {
            segments.Add(ParsePathSegment());
        }
        return new TypePathNode(doubleColon, segments.ToArray());
    }

    public VariableDeclarationNode ParseVariableDeclaration()
    {
        TokenNode kw = Expect(t => t is TokenType.KW_Let or TokenType.KW_Var);
        TokenNode name = Expect(TokenType.Identifier);
        TypeNode? type = null;
        if (Consume(TokenType.Colon, out var colon))
        {
            type = ParseType();
        }

        TokenNode equal = Expect(TokenType.Equals);
        ExpressionNode expr = ParseExpression();

        return new VariableDeclarationNode(kw, name, colon, type, equal, expr);
    }

    public VariableAssignmentNode ParseVariableAssignment()
    {
        TokenNode name = Expect(TokenType.Identifier);
        TokenNode equal = Expect(TokenType.Equals);
        ExpressionNode expr = ParseExpression();

        return new VariableAssignmentNode(name, equal, expr);
    }

    public BlockExpressionNode ParseBlockExpression()
    {
        TokenNode indent = Expect(TokenType.IND_Indent);
        TokenNode outdent;
        List<StatementNode> statements = new();
        ExpressionNode? expression;
        while (true)
        {
            var expr = ParseExpression();
            if (Consume(TokenType.IND_Outdent, out outdent!))
            {
                expression = expr;
                break;
            }
            var newline = Expect(TokenType.Newline);
            statements.Add(new StatementNode(expr, newline));
        }

        return new BlockExpressionNode(indent, statements.ToArray(), expression, outdent);
    }

    public IfExpressionNode ParseIfExpression()
    {
        TokenNode kwif = Expect(TokenType.KW_If);
        ExpressionNode ifExpr = ParseExpression();
        TokenNode colon = Expect(TokenType.Colon);
        BlockExpressionNode blockExpr = ParseBlockExpression();
        if (Consume(TokenType.KW_Else, out var kwelse))
        {
            if (Consume(TokenType.Colon, out var elseColon))
            {
                BlockExpressionNode elseExpr = ParseBlockExpression();
                return new IfExpressionNode(kwif, ifExpr, colon, blockExpr, kwelse, elseExpr, elseColon);
            } else
            {
                IfExpressionNode followup = ParseIfExpression();
                return new IfExpressionNode(kwif, ifExpr, colon, blockExpr, kwelse, followup, null);
            }
        }
        return new IfExpressionNode(kwif, ifExpr, colon, blockExpr, null, null, null);
    }

    public WhileExpressionNode ParseWhileExpression()
    {
        TokenNode kwwhile = Expect(TokenType.KW_While);
        ExpressionNode whileExpr = ParseExpression();
        TokenNode colon = Expect(TokenType.Colon);
        BlockExpressionNode block = ParseBlockExpression();

        return new WhileExpressionNode(kwwhile, whileExpr, colon, block);
    }

    public ExpressionNode ParseExpression()
    {
        if (PeekType(TokenType.KW_If))
        {
            return ParseIfExpression();
        }
        else if (PeekType(t => t is TokenType.KW_Let or TokenType.KW_Var))
        {
            return ParseVariableDeclaration();
        }
        else if (PeekType(TokenType.KW_While))
        {
            return ParseWhileExpression();
        }
        else if (PeekType(TokenType.DoubleColon))
        {
            return ParsePathExpression();
        }
        else if (PeekType(TokenType.Identifier))
        {
            if (PeekType(TokenType.LParen, skip: true))
            {
                return ParseFunctionCall();
            } else if (PeekType(TokenType.Equals, skip: true))
            {
                return ParseVariableAssignment();
            } else if (PeekType(TokenType.DoubleColon, skip: true))
            {
                return ParsePathExpression();
            }
        }
        return ParseTerm();
    }

    public UnitLiteralNode ParseUnitLiteral()
    {
        return new UnitLiteralNode(Expect(TokenType.UnitLiteral));
    }

    public PathExpressionNode ParsePathExpression()
    {
        TokenNode? doubleColon = Consume(TokenType.DoubleColon);
        List<PathSegmentNode> segments = new();
        while (PeekType(TokenType.Identifier) && PeekType(TokenType.DoubleColon, skip: true))
        {
            segments.Add(ParsePathSegment());
        }
        ExpressionNode expr = ParseExpression();
        return new PathExpressionNode(doubleColon, segments.ToArray(), expr);
    }

    public PathSegmentNode ParsePathSegment()
    {
        var ident = Expect(TokenType.Identifier);
        var period = Expect(TokenType.DoubleColon);

        return new PathSegmentNode(ident, period);
    }

    public FunctionCallNode ParseFunctionCall()
    {
        TokenNode identifier = Expect(TokenType.Identifier);
        ArgListNode arglist = ParseArgList();

        return new FunctionCallNode(identifier, arglist);
    }

    public ArgListNode ParseArgList()
    {
        TokenNode lparen = Expect(TokenType.LParen);
        TokenNode rparen;
        List<TokenNode> commas = new();
        List<ExpressionNode> args = new();

        if (!Consume(TokenType.RParen, out rparen!))
        {
            args.Add(ParseExpression());
            while (!Consume(TokenType.RParen, out rparen!))
            {
                commas.Add(Expect(TokenType.Comma));
                args.Add(ParseExpression());
            }
        }
            
        return new ArgListNode(lparen, args.ToArray(), commas.ToArray(), rparen);
    }

    public TermNode ParseTerm()
    {
        ItemNode left = ParseItem();
        List<ExpressionNode> itemlist = new();
        List<TokenNode> operators = new();
        while (true)
        {
            var op = ParseOptionalBinaryOp();
            if (op is not null)
            {
                operators.Add(op);
                itemlist.Add(ParseExpression());
            } else { break; }
        }
        return new TermNode(left, itemlist.ToArray(), operators.ToArray());
    }

    public ItemNode ParseItem()
    {
        // UnaryOp uop = ParseOptionalUnaryOp();
        return new ItemNode(ParseAtom());
    }

    public TokenNode? ParseOptionalBinaryOp()
    {
        Consume(t => t is TokenType.Plus or TokenType.Minus or TokenType.Asterisk or TokenType.Slash or TokenType.Period, out var tok);
        return tok;
    }

    public FactorNode ParseFactor()
    {
        AtomNode atom = ParseAtom();
        List<TokenNode> operators = new();
        List<AtomNode> atoms = new();
        while (true)
        {
            if (!PeekType(t => t is TokenType.Asterisk or TokenType.Slash))
            {
                break;
            }
            ExpectInto(t => t is TokenType.Asterisk or TokenType.Slash, operators);
            atoms.Add(ParseAtom());
        }
        return new FactorNode(atom, operators.ToArray(), atoms.ToArray());
    }

    public AtomNode ParseAtom()
    {
        if (PeekType(TokenType.LParen))
        {
            return ParseParenExpression();
        } else if (PeekType(TokenType.IntLiteral))
        {
            return ParseIntLiteral();
        } else if (PeekType(TokenType.UnitLiteral))
        {
            return ParseUnitLiteral();
        }
        else if (PeekType(TokenType.Identifier))
        {
            return ParseIdentifier();
        }
        else
        {
            throw new Exception($"Cannot use {Peek()?.Type} as expression");
        }
    }

    public IntLiteralNode ParseIntLiteral()
    {
        return new IntLiteralNode(Expect(TokenType.IntLiteral));
    }

    public IdentifierNode ParseIdentifier()
    {
        return new IdentifierNode(Expect(TokenType.Identifier));
    }

    public ParenExpressionNode ParseParenExpression()
    {
        TokenNode lparen = Expect(TokenType.LParen);
        ExpressionNode expr = ParseExpression();
        TokenNode rparen = Expect(TokenType.RParen);

        return new ParenExpressionNode(lparen, expr, rparen);
    }

    private void Consume()
    {
        if (Current is not null)
        {
            Advance();
        }
    }

    private bool ConsumeInto(TokenType type, List<TokenNode> tokens)
    {
        if (Current?.Type == type)
        {
            tokens.Add(new TokenNode(Current));
            Advance();
            return true;
        }
        return false;
    }

    private bool ConsumeInto(Predicate<Token> condition, List<TokenNode> tokens)
    {
        if (Current is not null && condition.Invoke(Current))
        {
            tokens.Add(new TokenNode(Current));
            Advance();
            return true;
        }
        return false;
    }

    private bool ConsumeInto(TokenType type, [NotNullWhen(true)] out TokenNode? token)
    {
        if (Current?.Type == type)
        {
            token = new TokenNode(Current);
            Advance();
            return true;
        }
        token = null;
        return false;
    }

    private bool ConsumeInto(Predicate<Token> condition, [NotNullWhen(true)] out TokenNode? token)
    {
        if (Current is not null && condition.Invoke(Current))
        {
            token = new TokenNode(Current);
            Advance();
            return true;
        }
        token = default;
        return false;
    }

    private bool Consume(TokenType type, [NotNullWhen(true)] out TokenNode? token)
    {
        if (Current?.Type == type)
        {
            token = new TokenNode(Current);
            Advance();
            return true;
        }
        token = null;
        return false;
    }

    private bool Consume(Predicate<TokenType> condition, [NotNullWhen(true)] out TokenNode? token)
    {
        if (Current is not null && condition.Invoke(Current.Type))
        {
            token = new TokenNode(Current);
            Advance();
            return true;
        }
        token = null;
        return false;
    }

    private TokenNode? Consume(TokenType type)
    {
        if (Current?.Type == type)
        {
            var token = new TokenNode(Current);
            Advance();
            return token;
        }
        return null;
    }

    private TokenNode Expect(TokenType type)
    {
        if (Current is null)
        {
            throw new Exception($"Expected token of type {type}, but got end of input.");
        }
        else if (Current.Type == type)
        {
            var node = new TokenNode(Current);
            Advance();
            return node;
        }
        else
        {
            throw new Exception($"Expected token of type {type}, but got token of type {Current.Type}");
        }
    }

    private TokenNode Expect(Predicate<TokenType> type)
    {
        if (Current is null)
        {
            throw new Exception($"Expected token matching pattern, but got end of input.");
        }
        else if (type.Invoke(Current.Type))
        {
            var c = Current;
            Advance();
            return new TokenNode(c);
        }
        else
        {
            throw new Exception($"Expected token matching pattern, but got token of type {Current.Type}");
        }
    }

    private void Expect(TokenType type, out TokenNode node)
    {
        if (Current is null)
        {
            throw new Exception($"Expected token of type {type}, but got end of input.");
        }
        else if (Current.Type == type)
        {
            node = new TokenNode(Current);
            Advance();
            return;
        }
        else
        {
            throw new Exception($"Expected token of type {type}, but got token of type {Current.Type}");
        }
    }

    private void Expect(Predicate<TokenType> type, out TokenNode node)
    {
        if (Current is null)
        {
            throw new Exception($"Expected token matching pattern, but got end of input.");
        }
        else if (type.Invoke(Current.Type))
        {
            node = new TokenNode(Current);
            Advance();
            return;
        }
        else
        {
            throw new Exception($"Expected token matching pattern, but got token of type {Current.Type}");
        }
    }

    private bool ExpectType(TokenType type)
    {
        if (Current is null)
        {
            throw new Exception($"Expected token of type {type}, but got end of input.");
        }
        else if (Current.Type == type)
        {
            Advance();
            return true;
        }
        else
        {
            throw new Exception($"Expected token of type {type}, but got token of type {Current.Type}");
        }
    }

    private void ExpectInto(TokenType type, List<TokenNode> tokens)
    {
        if (Current is null)
        {
            throw new Exception($"Expected token of type {type}, but got end of input.");
        }
        else if (Current.Type == type)
        {
            tokens.Add(new TokenNode(Current));
            Advance();
        } else
        {
            throw new Exception($"Expected token of type {type}, but got token of type {Current.Type}");
        }
    }

    private void ExpectInto(Predicate<TokenType> type, List<TokenNode> tokens)
    {
        if (Current is null)
        {
            throw new Exception($"Expected token matching pattern, but got end of input.");
        }
        else if (type.Invoke(Current.Type))
        {
            tokens.Add(new TokenNode(Current));
            Advance();
        }
        else
        {
            throw new Exception($"Expected token matching pattern, but got token of type {Current.Type}");
        }
    }

    private bool PeekType(TokenType type, bool skip = false)
    {
        if (skip ? Next?.Type == type : Current?.Type == type)
        {
            return true;
        }
        return false;
    }

    private bool PeekType(Predicate<TokenType> type)
    {
        if (Current is not null && type.Invoke(Current.Type))
        {
            return true;
        }
        return false;
    }

    private Token? Peek()
    {
        return Current;
    }

    private void Advance()
    {
        Console.WriteLine("Advancing beyond " + Current?.Type);
        Current = Next;
        if (tokens.MoveNext())
        {
            Next = tokens.Current;
        }
        else
        {
            Next = null;
        }
    }
}