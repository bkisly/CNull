using System.Text;
using CNull.Common.Configuration;
using CNull.Common.Extensions;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    public class CommentLexerState(ICodeSource source, IErrorHandler errorHandler, ICompilerConfiguration configuration) : LexerState(source, errorHandler, configuration)
    {
        private readonly StringBuilder _builder = new();

        public override bool TryBuildToken(out Token token)
        {
            while (Source.CurrentCharacter.IsWhiteSpace() && !Source.IsCurrentCharacterNewLine)
                Source.MoveToNext();

            var lengthCounter = 0;
            while (Source is { IsCurrentCharacterNewLine: false, CurrentCharacter: not null })
            {
                if (++lengthCounter > configuration.MaxCommentLength)
                    return TokenFailed(out token, new InvalidTokenLengthError(TokenPosition, configuration.MaxCommentLength), false);

                _builder.Append(Source.CurrentCharacter);
                Source.MoveToNext();
            }

            Source.MoveToNext();
            token = new Token<string>(_builder.ToString(), TokenType.Comment, TokenPosition);
            return true;
        }
    }
}
