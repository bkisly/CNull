using System.Text;
using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// Represents a state in which string literal is built.
    /// </summary>
    /// <param name="source"></param>
    public class StringLiteralLexerState(ICodeSource source) : LexerState(source)
    {
        private readonly StringBuilder _builder = new();

        public override bool TryBuildToken(out Token token)
        {
            if (Source.CurrentCharacter != '"')
                return TokenFailed(out token, false);

            Source.MoveToNext();
            var isEscapeSequence = false;

            // @TODO: check for excessively long string literals
            while (Source.CurrentCharacter != '"' || (isEscapeSequence && Source.CurrentCharacter == '"'))
            {
                if (!Source.CurrentCharacter.HasValue || Source.IsCurrentCharacterNewLine)
                    return TokenFailed(out token);

                if (isEscapeSequence)
                {
                    char sequenceCharacter = default;

                    if(!TokenHelpers.TryBuildEscapeSequence(Source.CurrentCharacter.Value, ref sequenceCharacter))
                        return TokenFailed(out token);

                    _builder.Append(sequenceCharacter);
                    isEscapeSequence = false;
                }
                else
                {
                    if (Source.CurrentCharacter == '\\')
                        isEscapeSequence = true;
                    else _builder.Append(Source.CurrentCharacter);
                }

                Source.MoveToNext();
            } 

            Source.MoveToNext();
            token = new Token<string>(_builder.ToString(), TokenType.StringLiteral);
            return true;
        }
    }
}
