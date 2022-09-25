namespace Uncertainty.Parsing.AST.Nodes;

public interface AstNode
{ }

public interface TopLevelStatementNode : AstNode
{ }

public interface ExpressionNode : AstNode 
{ }

public interface ElseExpression : AstNode
{ }

public interface AtomNode : AstNode
{ }