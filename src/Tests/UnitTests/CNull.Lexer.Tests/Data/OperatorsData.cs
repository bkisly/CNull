using CNull.Lexer.Constants;

namespace CNull.Lexer.Tests.Data
{
    internal class OperatorsData : TheoryData<string, bool, Token>
    {
        public OperatorsData()
        {
            foreach (var op in TokenHelpers.OperatorsAndPunctors)
                Add($"{op}something();", true, new Token<string>(op, TokenType.OperatorOrPunctor));

            Add(" something", false, Token.Unknown());
            Add("+++", true, new Token<string>("+", TokenType.OperatorOrPunctor));
            Add("-=", true, new Token<string>("-", TokenType.OperatorOrPunctor));
            Add("'asacassaa", false, Token.Unknown());
            Add("123123", false, Token.Unknown());
            Add("", false, Token.Unknown());
            Add("/=", true, new Token<string>("/", TokenType.OperatorOrPunctor));
            Add("!==>abcde", true, new Token<string>("!=", TokenType.OperatorOrPunctor));
        }
    }

    internal class OperatorsLastCharacterData : TheoryData<string, char?>
    {
        public OperatorsLastCharacterData()
        {
            foreach (var op in TokenHelpers.OperatorsAndPunctors)
                Add($"{op}something();", 's');

            Add(" something", ' ');
            Add("something.", '.');
            Add("+++", '+');
            Add("-=", '=');
            Add("'asacassaa", '\'');
            Add("123123", null);
            Add("", null);
            Add("&&", null);
            Add("&&&", '&');
            Add("!=/>abcde", '/');
        }
    }
}
