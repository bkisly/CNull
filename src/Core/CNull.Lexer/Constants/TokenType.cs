namespace CNull.Lexer.Constants
{
    /// <summary>
    /// Represents the type of a token.
    /// </summary>
    public enum TokenType
    {
        #region Keywords

        BoolKeyword,
        IntKeyword,
        FloatKeyword,
        StringKeyword,
        CharKeyword,
        DictKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,
        ContinueKeyword,
        BreakKeyword,
        VoidKeyword,
        ReturnKeyword,
        ThrowKeyword,
        TryKeyword,
        CatchKeyword,
        TrueKeyword,
        FalseKeyword,
        NullKeyword,
        ImportKeyword,

        #endregion

        Identifier,
        OperatorOrPunctor,
        StringLiteral,
        IntegerLiteral,
        FloatLiteral,
        CharLiteral,
        Comment,
        End,
    }
}
