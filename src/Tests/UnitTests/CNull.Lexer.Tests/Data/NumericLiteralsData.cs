using CNull.Lexer.Constants;

namespace CNull.Lexer.Tests.Data
{
    internal class IntegerLiteralsData : TheoryData<string, bool, Token>
    {
        public IntegerLiteralsData()
        {
            Add("150+230 ==\n1;", true, new Token<int>(150, TokenType.IntegerLiteral));
            Add("0\nsomething();", true, new Token<int>(0, TokenType.IntegerLiteral));
            Add("9999 9999", true, new Token<int>(9999, TokenType.IntegerLiteral));
            Add("2000", true, new Token<int>(2000, TokenType.IntegerLiteral));
            Add("2147483647", true, new Token<int>(2147483647, TokenType.IntegerLiteral));
            Add("2147483649", false, Token.Unknown());
            Add("012345", false, Token.Unknown());
            Add("00", false, Token.Unknown());
            Add("123abcde", true, new Token<int>(123, TokenType.IntegerLiteral));
            Add("", false, Token.Unknown());
            Add("-250", false, Token.Unknown());
            Add("123123123123123123123123123123;", false, Token.Unknown());
        }
    }

    internal class FloatingPointLiteralsData : TheoryData<string, bool, Token>
    {
        public FloatingPointLiteralsData()
        {
            Add("123.4567-123== 0;", true, new Token<float>(123.4567f, TokenType.FloatLiteral));
            Add("45.00000 ==123", true, new Token<float>(45f, TokenType.FloatLiteral));
            Add("0.2345", true, new Token<float>(.2345f, TokenType.FloatLiteral));
            Add("0.9223372036854775807", true, new Token<float>(.9223372036854775807f, TokenType.FloatLiteral));
            Add("0.999999999999999", true, new Token<float>(.999999999999999f, TokenType.FloatLiteral));
            Add("123123123123123123123123123123.123123123123123123123123123123;", false, Token.Unknown());
            Add("200.123123123123123123123123123123123123123123123123123123123123", false, Token.Unknown());
            Add("0.00000000000000000000000000000000000000000000000000000000000000000", false, Token.Unknown());
            Add(".2456", false, Token.Unknown());
            Add("-24.560aaaaa", false, Token.Unknown());
        }
    }

    internal class NumericStateFinishingCharacterData : TheoryData<string, char?>
    {
        public NumericStateFinishingCharacterData()
        {
            Add("150+230 ==\n1;", '+');
            Add("0\nsomething();", '\n');
            Add("9999 9999", ' ');
            Add("2000", null);
            Add("2147483647", null);
            Add("2147483649", null);
            Add("012345", null);
            Add("00", null);
            Add("123abcde", 'a');
            Add("", null);
            Add("-250", '-');
            Add("123123123123123123123123123123;", ';');
            Add("123.4567-123== 0;", '-');
            Add("45.00000 ==123", ' ');
            Add("0.2345", null);
            Add("0.9223372036854775807", null);
            Add("0.999999999999999", null);
            Add("123123123123123123123123123123.123123123123123123123123123123;", '.');
            Add("200.123123123123123123123123123123123123123123123123123123123123", null);
            Add("0.00000000000000000000000000000000000000000000000000000000000000000", null);
            Add(".2456", '.');
            Add("-24.560aaaaa", '-');
        }
    }
}
