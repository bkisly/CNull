namespace CNull.Lexer.Constants
{
    /// <summary>
    /// Contains constants and mappings for lexems.
    /// </summary>
    public static class TokenHelpers
    {
        /// <summary>
        /// Maps literal keywords to corresponding token types.
        /// </summary>
        public static readonly Dictionary<string, TokenType> KeywordsToTokenTypes = new()
        {
            { "bool", TokenType.BoolKeyword },
            { "int", TokenType.IntKeyword },
            { "float", TokenType.FloatKeyword },
            { "string", TokenType.StringKeyword },
            { "char", TokenType.CharKeyword },
            { "dict", TokenType.DictKeyword },
            { "if", TokenType.IfKeyword },
            { "else", TokenType.ElseKeyword },
            { "while", TokenType.WhileKeyword },
            { "continue", TokenType.ContinueKeyword },
            { "break", TokenType.BreakKeyword },
            { "void", TokenType.VoidKeyword },
            { "return", TokenType.ReturnKeyword },
            { "throw", TokenType.ThrowKeyword },
            { "try", TokenType.TryKeyword },
            { "catch", TokenType.CatchKeyword },
            { "true", TokenType.TrueKeyword },
            { "false", TokenType.FalseKeyword },
            { "null", TokenType.NullKeyword },
            { "import", TokenType.ImportKeyword },
        };

        /// <summary>
        /// Contains a collection of recognizable operators and punctors.
        /// </summary>
        public static readonly string[] OperatorsAndPunctors = {
            "+", "-", "*", "/", "%", "=",
            "{", "}", "(", ")", ".", ",", ";",
            "&&", "||", ">", "<", "<=", ">=", "==", "!=", "!", "?"
        };

        private static readonly char[] OperatorsFirstCharacters = OperatorsAndPunctors.Select(o => o.First()).Distinct().ToArray();

        /// <summary>
        /// Determines whether given character can terminate a token.
        /// </summary>
        /// <param name="c">Checked character.</param>
        /// <returns></returns>
        public static bool IsTokenTerminator(this char? c) =>
            !c.HasValue || (!char.IsLetter(c.Value) && !char.IsNumber(c.Value) && c.Value != '_');

        /// <summary>
        /// Determines whether given character can be a starting point to build an operator.
        /// </summary>
        /// <param name="c">Checked character.</param>
        /// <returns></returns>
        public static bool IsOperatorCandidate(this char? c) => c.HasValue && IsOperatorCandidate(c.Value);

        /// <summary>
        /// Determines whether given character can be a starting point to build an operator.
        /// </summary>
        /// <param name="c">Checked character.</param>
        /// <returns></returns>
        public static bool IsOperatorCandidate(this char c) => OperatorsFirstCharacters.Contains(c);
    }
}
