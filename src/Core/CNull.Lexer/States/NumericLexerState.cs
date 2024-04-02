using CNull.Common.Extensions;
using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// State in which integer or floating-point literal is built.
    /// </summary>
    public class NumericLexerState(ICodeSource source) : LexerState(source)
    {
        private int _integerPart;
        private long _fractionPart;

        public override bool TryBuildToken(out Token token)
        {
            if (!CurrentCharacter.HasValue)
                return TokenFailed(out token);

            while (!TokenHelpers.IsTokenTerminator(CurrentCharacter))
            {
                if (!char.IsDigit(CurrentCharacter.Value) || _integerPart > int.MaxValue / 10)
                    return TokenFailed(out token);

                _integerPart = _integerPart * 10 + CurrentCharacter.Value - '0';
                Source.MoveToNext();
            }

            if (CurrentCharacter.Value != '.')
            {
                token = new Token<int>(_integerPart, TokenType.IntegerLiteral);
                return true;
            }

            Source.MoveToNext();
            while (!TokenHelpers.IsTokenTerminator(CurrentCharacter))
            {
                if (!char.IsDigit(CurrentCharacter.Value) || _fractionPart > long.MaxValue / 10)
                    return TokenFailed(out token);

                _fractionPart = _fractionPart * 10 + CurrentCharacter.Value - '0';
                Source.MoveToNext();
            }

            var fractionValue = _fractionPart / (float)Math.Pow(10, _fractionPart.Length());
            token = new Token<float>(_integerPart + fractionValue, TokenType.FloatLiteral);
            return true;
        }
    }
}
