using CNull.Common;

namespace CNull.Parser.Productions
{
    /// <summary>
    /// Represents a general type of an expression.
    /// </summary>
    public interface IExpression : ISyntacticProduction;

    public interface IBinaryExpression : IExpression
    {
        IExpression LeftFactor { get; }
        IExpression RightFactor { get; }
    }

    public interface IUnaryExpression : IExpression
    {
        IExpression Expression { get; }
    }

    public delegate IBinaryExpression BinaryExpressionFactory(IExpression left, IExpression right, Position position);

    public delegate IUnaryExpression UnaryExpressionFactory(IExpression expression, Position position);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record OrExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record AndExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record GreaterThanExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record GreaterThanOrEqualExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record EqualExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record NotEqualExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record LessThanExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record LessThanOrEqualExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record AdditionExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record SubtractionExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record MultiplicationExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record DivisionExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="LeftFactor"></param>
    /// <param name="RightFactor"></param>
    /// <param name="Position"></param>
    public record ModuloExpression(IExpression LeftFactor, IExpression RightFactor, Position Position) : IBinaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Expression"></param>
    /// <param name="Position"></param>
    public record BooleanNegationExpression(IExpression Expression, Position Position) : IUnaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Expression"></param>
    /// <param name="Position"></param>
    public record NegationExpression(IExpression Expression, Position Position) : IUnaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Expression"></param>
    /// <param name="Position"></param>
    public record NullCheckExpression(IExpression Expression, Position Position) : IUnaryExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Value"></param>
    /// <param name="Position"></param>
    public record LiteralExpression<T>(T? Value, Position Position) : IExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="InnerExpression"></param>
    /// <param name="Position"></param>
    public record ParenthesisedExpression(IExpression InnerExpression, Position Position) : IExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Identifier"></param>
    /// <param name="Position"></param>
    /// <param name="Arguments"></param>
    public record IdentifierOrCallExpression(string Identifier, Position Position, IEnumerable<IExpression>? Arguments = null) : IExpression;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="AccessedMember"></param>
    /// <param name="Position"></param>
    public record MemberAccessExpression(IExpression AccessedMember, Position Position, MemberAccessExpression? ParentMember = null) : IExpression;
}
