namespace CNull.Lexer.Constants
{
    /// <summary>
    /// Represents the type of token.
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

        #region Operators and punctors

        PlusOperator,
        MinusOperator,
        AsteriskOperator,
        SlashOperator,
        PercentOperator,
        AssignmentOperator,
        OpenBlockOperator,
        CloseBlockOperator,
        LeftParenthesisOperator,
        RightParenthesisOperator,
        DotOperator,
        CommaOperator,
        SemicolonOperator,
        AndOperator,
        OrOperator,
        GreaterThanOperator,
        LessThanOperator,
        GreaterThanOrEqualOperator,
        LessThanOrEqualOperator,
        EqualOperator,
        NotEqualOperator,
        NegationOperator,
        IsNullOperator,

        #endregion

        Identifier,
        StringLiteral,
        IntegerLiteral,
        FloatLiteral,
        CharLiteral,
        Comment,
        End,
        Unknown,
    }
}
