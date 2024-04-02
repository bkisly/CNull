using System.Numerics;
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
        private long _integerPart;
        private long _fractionPart;

        public override bool TryBuildToken(out Token token)
        {
            if (TokenHelpers.IsTokenTerminator(CurrentCharacter))
                return TokenFailed(out token);

            if (CurrentCharacter is '0')
            {
                ProcessDigit(ref _integerPart, CurrentCharacter.Value);
                if (CurrentCharacter is '0')
                    return TokenFailed(out token);
            }

            if(!TryBuildNumberPart(ref _integerPart, int.MaxValue))
                return TokenFailed(out token);

            if (CurrentCharacter is not '.')
            {
                token = new Token<int>((int)_integerPart, TokenType.IntegerLiteral);
                return true;
            }

            Source.MoveToNext();

            if(!TryBuildNumberPart(ref _fractionPart, long.MaxValue))
                return TokenFailed(out token);

            var fractionValue = _fractionPart / (float)Math.Pow(10, _fractionPart.Length());
            token = new Token<float>(_integerPart + fractionValue, TokenType.FloatLiteral);
            return true;
        }

        private bool TryBuildNumberPart(ref long partValue, long maxValue)
        {
            while (!TokenHelpers.IsTokenTerminator(CurrentCharacter))
            {
                if (!char.IsAsciiDigit(CurrentCharacter!.Value) || partValue > maxValue / 10)
                    return false;

                ProcessDigit(ref partValue, CurrentCharacter.Value);
            }

            return true;
        }

        private void ProcessDigit(ref long partValue, char character)
        {
            partValue = partValue * 10 + character - '0';
            Source.MoveToNext();
        }
    }
}
