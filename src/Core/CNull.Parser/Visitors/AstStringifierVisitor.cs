using System.Text;
using CNull.Parser.Productions;

namespace CNull.Parser.Visitors
{
    /// <summary>
    /// Visitor capable of converting AST to string.
    /// </summary>
    public class AstStringifierVisitor : IAstVisitor
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

            AppendLineWithInner("Import directives:", program.ImportDirectives);
            AppendLineWithInner("Function definitions:", program.FunctionDefinitions);

            AppendLine("End of the program.");
        }

        public void Visit(ImportDirective directive)
        {
            AppendLine($"Import directive at {directive.Position}");
            _indent++;
            AppendLine($"Module name: {directive.ModuleName}");
            AppendLine($"Function name: {directive.FunctionName}");
            _indent--;
        }

        public void Visit(FunctionDefinition functionDefinition)
        {
            AppendLine($"Function definition at {functionDefinition.Position}");
            _indent++;

            AppendLineWithInner("Return type:", functionDefinition.ReturnType);
            AppendLine($"Name: {functionDefinition.Name}");
            AppendLineWithInner("Parameters", functionDefinition.Parameters);
            AppendLineWithInner("Function body:", functionDefinition.FunctionBody);

            _indent--;
        }

        public void Visit(Parameter parameter)
        {
            AppendLine($"Parameter at {parameter.Position}");
            _indent++;

            AppendLineWithInner("Type:", parameter.Type);
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
            AppendLine($"{dictionaryType.Type} type at {dictionaryType.Position}");
            _indent++;

            AppendLineWithInner("Key type:", dictionaryType.KeyType);
            AppendLineWithInner("Value type:", dictionaryType.ValueType);

            _indent--;
        }

        public void Visit(BlockStatement statement)
        {
            AppendLineWithInner($"Block statement at {statement.Position}", statement.StatementsList);
        }

        public void Visit(VariableDeclaration variableDeclaration)
        {
            AppendLine($"Variable declaration at {variableDeclaration.Position}");
            _indent++;

            AppendLineWithInner("Type:", variableDeclaration.Type);
            AppendLine($"Name: {variableDeclaration.Name}");
            AppendLineWithInner("Initialization expression:", variableDeclaration.InitializationExpression);

            _indent--;
        }

        public void Visit(ExpressionStatement expressionStatement)
        {
            AppendLine($"Expression statement at: {expressionStatement.Position}");
            _indent++;

            AppendLineWithInner("Expression:", expressionStatement.Expression);
            AppendLineWithInner("Assignment value:", expressionStatement.AssignmentValue);

            _indent--;
        }

        public void Visit(IfStatement ifStatement)
        {
            AppendLine($"If statement at {ifStatement.Position}");
            _indent++;

            AppendLineWithInner("Conditional expression:", ifStatement.BooleanExpression);
            AppendLineWithInner("Statement body:", ifStatement.Body);
            AppendLineWithInner("Else if statement:", ifStatement.ElseIfStatement);
            AppendLineWithInner("Else block:", ifStatement.ElseBlock);

            _indent--;
        }

        public void Visit(WhileStatement whileStatement)
        {
            AppendLine($"While statement at: {whileStatement.Position}");
            _indent++;

            AppendLineWithInner("Conditional expression:", whileStatement.BooleanExpression);
            AppendLineWithInner("Statement body:", whileStatement.Body);

            _indent--;
        }

        public void Visit(TryStatement tryCatchStatement)
        {
            AppendLine($"Try-catch statement at: {tryCatchStatement.Position}");
            _indent++;

            AppendLineWithInner("Try body:", tryCatchStatement.TryBlock);
            AppendLineWithInner("Catch clauses:", tryCatchStatement.CatchClauses);

            _indent--;
        }

        public void Visit(CatchClause catchClause)
        {
            AppendLine($"Catch clause at: {catchClause.Position}");
            _indent++;

            AppendLine($"Identifier: {catchClause.Identifier}");
            AppendLineWithInner("Filter expression:", catchClause.FilterExpression);
            AppendLineWithInner("Catch body:", catchClause.Body);

            _indent--;
        }

        public void Visit(ContinueStatement continueStatement)
        {
            AppendLine($"Continue statement at: {continueStatement.Position}");
        }

        public void Visit(BreakStatement breakStatement)
        {
            AppendLine($"Break statement at: {breakStatement.Position}");
        }

        public void Visit(ThrowStatement throwStatement)
        {
            AppendLine($"Throw statement at: {throwStatement.Position}");
            _indent++;

            AppendLine($"Thrown string: {throwStatement.Message}");

            _indent--;
        }

        public void Visit(ReturnStatement returnStatement)
        {
            AppendLine($"Return statement at: {returnStatement.Position}");
            _indent++;

            AppendLineWithInner("Returned expression:", returnStatement.ReturnExpression);

            _indent--;
        }

        public void Visit(OrExpression orExpression)
        {
            AppendLine($"Or expression at: {orExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", orExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", orExpression.RightFactor);

            _indent--;
        }

        public void Visit(AndExpression andExpression)
        {
            AppendLine($"And expression at: {andExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", andExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", andExpression.RightFactor);

            _indent--;
        }

        public void Visit(GreaterThanExpression greaterThanExpression)
        {
            AppendLine($"Greater than expression at: {greaterThanExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", greaterThanExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", greaterThanExpression.RightFactor);

            _indent--;
        }

        public void Visit(LessThanExpression lessThanExpression)
        {
            AppendLine($"Less than expression at: {lessThanExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", lessThanExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", lessThanExpression.RightFactor);

            _indent--;
        }

        public void Visit(GreaterThanOrEqualExpression greaterThanOrEqualExpression)
        {
            AppendLine($"Greater than or equal expression at: {greaterThanOrEqualExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", greaterThanOrEqualExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", greaterThanOrEqualExpression.RightFactor);

            _indent--;
        }

        public void Visit(LessThanOrEqualExpression lessThanOrEqualExpression)
        {
            AppendLine($"Less than or equal expression at: {lessThanOrEqualExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", lessThanOrEqualExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", lessThanOrEqualExpression.RightFactor);

            _indent--;
        }

        public void Visit(EqualExpression equalExpression)
        {
            AppendLine($"Equal expression at: {equalExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", equalExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", equalExpression.RightFactor);

            _indent--;
        }

        public void Visit(NotEqualExpression notEqualExpression)
        {
            AppendLine($"Not equal expression at: {notEqualExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", notEqualExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", notEqualExpression.RightFactor);

            _indent--;
        }

        public void Visit(AdditionExpression additionExpression)
        {
            AppendLine($"Addition expression at: {additionExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", additionExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", additionExpression.RightFactor);

            _indent--;
        }

        public void Visit(SubtractionExpression subtractionExpression)
        {
            AppendLine($"Subtraction expression at: {subtractionExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", subtractionExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", subtractionExpression.RightFactor);

            _indent--;
        }

        public void Visit(MultiplicationExpression multiplicationExpression)
        {
            AppendLine($"Multiplication expression at: {multiplicationExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", multiplicationExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", multiplicationExpression.RightFactor);

            _indent--;
        }

        public void Visit(DivisionExpression divideExpression)
        {
            AppendLine($"Division expression at: {divideExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", divideExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", divideExpression.RightFactor);

            _indent--;
        }

        public void Visit(ModuloExpression moduloExpression)
        {
            AppendLine($"Modulo expression at: {moduloExpression.Position}");
            _indent++;

            AppendLineWithInner("Left logic factor:", moduloExpression.LeftFactor);
            AppendLineWithInner("Right logic factor:", moduloExpression.RightFactor);

            _indent--;
        }

        public void Visit(BooleanNegationExpression booleanNegationExpression)
        {
            AppendLine($"Boolean negation expression at: {booleanNegationExpression.Position}");
            _indent++;

            AppendLineWithInner("Negated expression:", booleanNegationExpression.Expression);

            _indent--;
        }

        public void Visit(NegationExpression negationExpression)
        {
            AppendLine($"Mathematical negation expression at: {negationExpression.Position}");
            _indent++;

            AppendLineWithInner("Negated expression:", negationExpression.Expression);

            _indent--;
        }

        public void Visit(NullCheckExpression nullCheckExpression)
        {
            AppendLine($"Null-check expression at: {nullCheckExpression.Position}");
            _indent++;

            AppendLineWithInner("Checked expression:", nullCheckExpression.Expression);

            _indent--;
        }

        public void Visit<T>(LiteralExpression<T> literalExpression)
        {
            AppendLine($"Literal expression at: {literalExpression.Position}");
            _indent++;

            AppendLine($"Value: {literalExpression.Value?.ToString() ?? "null"}");

            _indent--;
        }

        public void Visit(ParenthesisedExpression parenthesisedExpression)
        {
            AppendLine($"Parenthesised expression at: {parenthesisedExpression.Position}");
            _indent++;

            AppendLineWithInner("Inner expression:", parenthesisedExpression.InnerExpression);

            _indent--;
        }

        public void Visit(IdentifierExpression identifierExpression)
        {
            AppendLine($"Identifier expression at: {identifierExpression.Position}");
            _indent++;

            AppendLine($"Identifier: {identifierExpression.Identifier}");

            _indent--;
        }

        public void Visit(CallExpression callExpression)
        {
            AppendLine($"Function call expression at: {callExpression.Position}");
            _indent++;

            AppendLine($"Function name: {callExpression.FunctionName}");
            AppendLineWithInner("Arguments:", callExpression.Arguments);

            _indent--;
        }

        public void Visit(MemberAccessExpression memberAccessExpression)
        {
            AppendLine($"Member access expression at: {memberAccessExpression.Position}");
            _indent++;

            AppendLineWithInner("Parent member:", memberAccessExpression.ParentMember);

            _indent--;
        }

        private void AppendLine(string obj)
        {
            for (var i = 0; i < _indent; i++)
                _builder.Append("  ");
            _builder.AppendLine(obj);
        }

        private void AppendLineWithInner(string header, IAstVisitable? visitable)
        {
            if (visitable == null)
                return;

            AppendLine(header);
            _indent++;
            visitable.Accept(this);
            _indent--;
        }

        private void AppendLineWithInner(string header, IEnumerable<IAstVisitable?> visitables)
        {
            if(!visitables.Any())
                return;

            AppendLine(header);
            _indent++;

            foreach (var astVisitable in visitables)
            {
                astVisitable?.Accept(this);
            }

            _indent--;
        }
    }
}
