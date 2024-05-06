using CNull.Lexer.Constants;

namespace CNull.Lexer.Extensions
{
    public static class TokenTypeExtensions
    {
        public static string ToLiteralString(this TokenType type)
        {
            var mappingUnion = TokenHelpers.OperatorsToTokenTypes.Union(TokenHelpers.KeywordsToTokenTypes).ToDictionary();

            return mappingUnion.ContainsValue(type) 
                ? mappingUnion.First(p => p.Value == type).Key
                : type.ToString();
        }
    }
}
