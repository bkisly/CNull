using CNull.Common;
using CNull.Parser.Enums;

namespace CNull.Parser.Productions
{
    /// <summary>
    /// Syntactic production that represents a type, that can be used in variable declarations.
    /// </summary>
    public interface IDeclarableType : ISyntacticProduction
    {
        /// <summary>
        /// Indicates whether the type is primitive.
        /// </summary>
        bool IsPrimitive { get; }

        /// <summary>
        /// Type specifier.
        /// </summary>
        Types Type { get; }
    }

    /// <summary>
    /// Syntactic production that represents a returnable type.
    /// </summary>
    public record ReturnType(Position Position) : ISyntacticProduction
    {
        /// <summary>
        /// Specifier for the type.
        /// </summary>
        public virtual Types Type => Types.Void;
    }

    /// <summary>
    /// Syntactic production that represents a primitive type.
    /// </summary>
    /// <param name="TypeSpecifier">Specifier for the type.</param>
    public record PrimitiveType(PrimitiveTypes TypeSpecifier, Position Position) : ReturnType(Position), IDeclarableType
    {
        public override Types Type => (Types)TypeSpecifier;
        public bool IsPrimitive => true;
    }

    /// <summary>
    /// Syntactic production that represents the dictionary type.
    /// </summary>
    /// <param name="KeyType">Type of the key.</param>
    /// <param name="ValueType">Type of the value.</param>
    public record DictionaryType(PrimitiveType KeyType, PrimitiveType ValueType, Position Position) : ReturnType(Position), IDeclarableType
    {
        public override Types Type => Types.Dictionary;
        public bool IsPrimitive => false;
    }
}
