﻿using CNull.Common;
using CNull.Parser.Visitors;

namespace CNull.Parser.Productions
{
    /// <summary>
    /// Syntactic production, which represents a list of statements grouped in a block.
    /// </summary>
    /// <param name="StatementsList">The list of statements.</param>
    public record BlockStatement(IEnumerable<IBasicStatement> StatementsList, Position Position) : ISyntacticProduction
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Represents a general type of single statement.
    /// </summary>
    public interface IBasicStatement : ISyntacticProduction;

    /// <summary>
    /// Syntactic production which represents the declaration of a variable with optional assignment.
    /// </summary>
    /// <param name="Type">Type of the variable.</param>
    /// <param name="Name">Name of the variable.</param>
    /// <param name="InitializationExpression">Expression which returns a value to initialize the variable.</param>
    public record VariableDeclaration(IDeclarableType Type, string Name, Position Position, IExpression? InitializationExpression = null) : IBasicStatement
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents an expression encapsulated into a statement with optional assignment.
    /// </summary>
    /// <param name="Expression">Basic expression of the statement.</param>
    /// <param name="AssignmentValue">Value to assign.</param>
    public record ExpressionStatement(IExpression Expression, Position Position, IExpression? AssignmentValue = null) : IBasicStatement
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which recursively represents the if-else statement.
    /// </summary>
    /// <param name="BooleanExpression"></param>
    /// <param name="Body"></param>
    public record IfStatement(IExpression BooleanExpression, BlockStatement Body, IfStatement? ElseIfStatement,
        BlockStatement? ElseBlock, Position Position) : IBasicStatement
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the while loop statement.
    /// </summary>
    /// <param name="BooleanExpression">Expression determining the condition of the continuation.</param>
    /// <param name="Body">Body of the loop.</param>
    public record WhileStatement(IExpression BooleanExpression, BlockStatement Body, Position Position) : IBasicStatement
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents a try-catch statement.
    /// </summary>
    /// <param name="TryBlock">Block of statements to try.</param>
    /// <param name="CatchClauses">List of catch clauses.</param>
    public record TryStatement(BlockStatement TryBlock, IEnumerable<CatchClause> CatchClauses, Position Position) : IBasicStatement
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Helper production to represent the catch clause.
    /// </summary>
    /// <param name="Identifier">Identifier of the variable, which will contain the catched message.</param>
    /// <param name="FilterExpression">Expression which filters the clause.</param>
    /// <param name="Body">Clause body.</param>
    public record CatchClause(string Identifier, IExpression? FilterExpression, BlockStatement Body, Position Position) : ISyntacticProduction
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents a continuation statement inside a loop.
    /// </summary>
    public record ContinueStatement(Position Position) : IBasicStatement
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents loop break.
    /// </summary>
    public record BreakStatement(Position Position) : IBasicStatement
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents throwing an exception.
    /// </summary>
    /// <param name="Message">Message to throw.</param>
    public record ThrowStatement(string Message, Position Position) : IBasicStatement
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents return statement inside a function.
    /// </summary>
    /// <param name="ReturnExpression">Expression to return from the function.</param>
    public record ReturnStatement(IExpression? ReturnExpression, Position Position) : IBasicStatement
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
