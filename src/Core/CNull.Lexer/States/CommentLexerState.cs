using System.Text;
using CNull.Common.Extensions;
using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
using CNull.Lexer.ServicesContainers;

namespace CNull.Lexer.States
{
    /// <summary>
    /// State in which comment token is built.
    /// </summary>
    public class CommentLexerState(ILexerStateServicesContainer servicesContainer) : LexerState(servicesContainer)
    {
        private readonly StringBuilder _builder = new();

        public override Token BuildToken()
        {
            while (Source.CurrentCharacter.IsWhiteSpace() && !Source.IsCurrentCharacterNewLine)
                Source.MoveToNext();

            var lengthCounter = 0;
            while (Source is { IsCurrentCharacterNewLine: false, CurrentCharacter: not null })
            {
                if (++lengthCounter > Configuration.MaxCommentLength)
                    return TokenFailed(new InvalidTokenLengthError(TokenPosition, Configuration.MaxCommentLength), false);

                _builder.Append(Source.CurrentCharacter);
                Source.MoveToNext();
            }

            Source.MoveToNext();
            return new Token<string>(_builder.ToString(), TokenType.Comment, TokenPosition);
        }
    }
}
