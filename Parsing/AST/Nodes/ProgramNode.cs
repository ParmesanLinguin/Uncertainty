namespace Uncertainty.Parsing.AST.Nodes;

public record class ProgramNode : AstNode
{
    public TopLevelStatementNode[] Statements { get; init; }

    public TokenNode EOF { get; init; }

    public ProgramNode(TopLevelStatementNode[] statements, TokenNode eof)
    {
        Statements = statements;
        EOF = eof;
    }
}
