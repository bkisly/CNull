using System.Text;
using CNull.Parser.Productions;

namespace CNull.Parser.Visitors
{
    /// <summary>
    /// Visitor capable of converting AST to string.
    /// </summary>
    internal class AstStringifierVisitor : IAstVisitor
    {
        private readonly StringBuilder _builder = new();
        private int _indent;

        /// <summary>
        /// Converts the program AST to string.
        /// </summary>
        /// <param name="program">Program to process.</param>
        public string GetString(Program program)
        {
            _builder.Clear();
            program.Accept(this);
            return _builder.ToString();
        }

        public void Visit(Program program)
        {
            _builder.Clear();
            AppendLine("AST of the program.");
            AppendLine("Import directives:");

            _indent++;
            foreach (var importDirective in program.ImportDirectives)
                importDirective.Accept(this);
            _indent--;

            AppendLine("Function definitions:");

            _indent++;
            foreach (var functionDefinition in program.FunctionDefinitions)
                functionDefinition.Accept(this);
            _indent--;

            AppendLine("End of the program.");
        }

        public void Visit(ImportDirective directive)
        {
            AppendLine($"Import directive at {directive.Position}.");
            _indent++;
            AppendLine($"Module name: {directive.ModuleName}");
            AppendLine($"Function name: {directive.FunctionName}");
            _indent--;
        }

        public void Visit(FunctionDefinition functionDefinition)
        {
            AppendLine($"Function definition at {functionDefinition.Position}");
            _indent++;

            AppendLine($"Return type:");

            _indent++;
            functionDefinition.ReturnType.Accept(this);
            _indent--;

            AppendLine($"Name: {functionDefinition.Name}");

            AppendLine($"Parameters:");
            _indent++;
            foreach (var parameter in functionDefinition.Parameters)
                parameter.Accept(this);
            _indent--;

            AppendLine("Function body:");
            _indent++;
            functionDefinition.FunctionBody.Accept(this);
            _indent--;

            _indent--;
        }

        public void Visit(Parameter parameter)
        {
            AppendLine($"Parameter at {parameter.Position}");
            _indent++;

            AppendLine("Type:");
            _indent++;
            parameter.Type.Accept(this);
            _indent--;

            AppendLine($"Name: {parameter.Name}");

            _indent--;
        }

        public void Visit(ReturnType returnType)
        {
            AppendLine($"{returnType.Type} type at {returnType.Position}");
        }

        public void Visit(PrimitiveType primitiveType)
        {
            AppendLine($"{primitiveType.TypeSpecifier} type at {primitiveType.Position}");
        }

        public void Visit(DictionaryType dictionaryType)
        {
            AppendLine($"{dictionaryType.Type} type at {dictionaryType.Type}");
            _indent++;

            AppendLine($"Key type:");
            _indent++;
            dictionaryType.KeyType.Accept(this);
            _indent--;

            AppendLine("Value type:");
            _indent--;
            dictionaryType.ValueType.Accept(this);
            _indent--;

            _indent--;
        }

        public void Visit(BlockStatement statement)
        {
            AppendLine($"Block statement at {statement.Position}");
            _indent++;

            foreach (var basicStatement in statement.StatementsList)
                basicStatement.Accept(this);

            _indent--;
        }

        public void Visit(VariableDeclaration variableDeclaration)
        {
            throw new NotImplementedException();
        }

        public void Visit(ExpressionStatement expressionStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(IfStatement ifStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(WhileStatement whileStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(TryStatement tryCatchStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(CatchClause catchClause)
        {
            throw new NotImplementedException();
        }

        public void Visit(ContinueStatement continueStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(BreakStatement breakStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(ThrowStatement throwStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(ReturnStatement returnStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(OrExpression orExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(AndExpression andExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(GreaterThanExpression greaterThanExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(LessThanExpression lessThanExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(GreaterThanOrEqualExpression greaterThanOrEqualExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(LessThanOrEqualExpression lessThanOrEqualExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(EqualExpression equalExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(NotEqualExpression notEqualExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(AdditionExpression additionExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(SubtractionExpression subtractionExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(MultiplicationExpression multiplicationExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(DivisionExpression divideExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(ModuloExpression moduloExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(BooleanNegationExpression booleanNegationExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(NegationExpression negationExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(NullCheckExpression nullCheckExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit<T>(LiteralExpression<T> literalExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(ParenthesisedExpression parenthesisedExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(IdentifierExpression identifierExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(CallExpression callExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(MemberAccessExpression memberAccessExpression)
        {
            throw new NotImplementedException();
        }

        private void AppendLine(string obj)
        {
            for (var i = 0; i < _indent; i++)
                _builder.Append('\t');
            _builder.AppendLine(obj);
        }
    }
}
