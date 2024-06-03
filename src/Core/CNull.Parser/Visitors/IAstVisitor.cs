using CNull.Parser.Productions;

namespace CNull.Parser.Visitors
{
    /// <summary>
    /// Represents a visitor, which can visit AST (abstract syntax tree) elements.
    /// </summary>
    public interface IAstVisitor
    {
        void Visit(Program program);
        void Visit(ImportDirective directive);
        void Visit(FunctionDefinition functionDefinition);
        void Visit(StandardLibraryFunction standardLibraryFunction);
        void Visit(EmbeddedFunction embeddedFunction);
        void Visit(Parameter parameter);

        void Visit(ReturnType returnType);
        void Visit(PrimitiveType primitiveType);
        void Visit(DictionaryType dictionaryType);

        void Visit(BlockStatement blockStatement);
        void Visit(VariableDeclaration variableDeclaration);
        void Visit(ExpressionStatement expressionStatement);
        void Visit(IfStatement ifStatement);
        void Visit(WhileStatement whileStatement);
        void Visit(TryStatement tryCatchStatement);
        void Visit(CatchClause catchClause);
        void Visit(ContinueStatement continueStatement);
        void Visit(BreakStatement breakStatement);
        void Visit(ThrowStatement throwStatement);
        void Visit(ReturnStatement returnStatement);

        void Visit(OrExpression orExpression);
        void Visit(AndExpression andExpression);
        void Visit(GreaterThanExpression greaterThanExpression);
        void Visit(LessThanExpression lessThanExpression);
        void Visit(GreaterThanOrEqualExpression greaterThanOrEqualExpression);
        void Visit(LessThanOrEqualExpression lessThanOrEqualExpression);
        void Visit(EqualExpression equalExpression);
        void Visit(NotEqualExpression notEqualExpression);

        void Visit(AdditionExpression additionExpression);
        void Visit(SubtractionExpression subtractionExpression);
        void Visit(MultiplicationExpression multiplicationExpression);
        void Visit(DivisionExpression divideExpression);
        void Visit(ModuloExpression moduloExpression);

        void Visit(BooleanNegationExpression booleanNegationExpression);
        void Visit(NegationExpression negationExpression);
        void Visit(NullCheckExpression nullCheckExpression);

        void Visit<T>(LiteralExpression<T> literalExpression);
        void Visit(IdentifierExpression identifierExpression);
        void Visit(CallExpression callExpression);
    }

    public interface IAstVisitable
    {
        void Accept(IAstVisitor visitor);
    }
}
