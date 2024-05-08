using CNull.Common;
using CNull.Parser.Visitors;

namespace CNull.Parser.Productions
{
    /// <summary>
    /// Represents a general type of an expression.
    /// </summary>
    public interface IExpression : ISyntacticProduction;

    /// <summary>
    /// Represents a general type of a binary expression.
    /// </summary>
    public interface IBinaryExpression : IExpression
    {
        /// <summary>
        /// Left logic factor of the expression.
        /// </summary>
        IExpression LeftFactor { get; }

        /// <summary>
        /// Right logic factor of the expression.
        /// </summary>
        IExpression RightFactor { get; }
    }

    /// <summary>
    /// Represents a general type of an unary expression.
    /// </summary>
    public interface IUnaryExpression : IExpression
    {
        /// <summary>
        /// The expression affected by the unary operation.
        /// </summary>
        IExpression Expression { get; }
    }

    /// <summary>
    /// Defines a factory for binary expressions.
    /// </summary>
    /// <param name="left">Left logic factor.</param>
    /// <param name="right">Right logic factor.</param>
    /// <param name="position">Position of the expression.</param>
    /// <returns></returns>
    public delegate IBinaryExpression BinaryExpressionFactory(IExpression left, IExpression right, Position position);

    /// <summary>
    /// Defines a factory for unary expressions.
    /// </summary>
    /// <param name="expression">The expression affected by the unary operation.</param>
    /// <param name="position">Position of the expression.</param>
    /// <returns></returns>
    public delegate IUnaryExpression UnaryExpressionFactory(IExpression expression, Position position);

    /// <summary>
    /// Syntactic production which represents the logical OR expression.
    /// </summary>
    public record OrExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the logical AND expression.
    /// </summary>
    public record AndExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the relational greater than expression.
    /// </summary>
    public record GreaterThanExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the relational greater than or equal expression.
    /// </summary>
    public record GreaterThanOrEqualExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the relational equal expression.
    /// </summary>
    public record EqualExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the relational not equal expression.
    /// </summary>
    public record NotEqualExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the relational less than expression.
    /// </summary>
    public record LessThanExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the relational less than or equal expression.
    /// </summary>
    public record LessThanOrEqualExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the addition binary expression.
    /// </summary>
    public record AdditionExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the subtraction binary expression.
    /// </summary>
    public record SubtractionExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the multiplication binary expression.
    /// </summary>
    public record MultiplicationExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the division binary expression.
    /// </summary>
    public record DivisionExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the modulo binary expression.
    /// </summary>
    public record ModuloExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the unary boolean negation expression.
    /// </summary>
    public record BooleanNegationExpression(IExpression Expression, Position Position) : IUnaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the unary mathematical negation expression.
    /// </summary>
    public record NegationExpression(IExpression Expression, Position Position) : IUnaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the unary null checking expression.
    /// </summary>
    public record NullCheckExpression(IExpression Expression, Position Position) : IUnaryExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the literal.
    /// </summary>
    public record LiteralExpression<T>(T? Value, Position Position) : IExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the parenthesised expression.
    /// </summary>
    public record ParenthesisedExpression(IExpression InnerExpression, Position Position) : IExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the reference to an identifier.
    /// </summary>
    public record IdentifierExpression(string Identifier, Position Position) : IExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the function call.
    /// </summary>
    public record CallExpression(string FunctionName, IEnumerable<IExpression> Arguments, Position Position) : IExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents the member access.
    /// </summary>
    public record MemberAccessExpression(IExpression AccessedMember, Position Position, MemberAccessExpression? ParentMember = null) : IExpression
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
