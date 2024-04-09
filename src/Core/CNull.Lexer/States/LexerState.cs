using CNull.Common;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// Base class for lexer states that interact with the source.
    /// </summary>
    /// <param name="source"></param>
    public abstract class LexerState(ICodeSource source, IErrorHandler errorHandler) : ILexerState
    {
        protected ICodeSource Source = source;
        protected char? CurrentCharacter => Source.CurrentCharacter;
        protected readonly Position TokenPosition = source.Position;

        public abstract bool TryBuildToken(out Token token);

        protected bool TokenFailed(out Token token, ICompilationError error, bool shouldSkipToken = true)
        {
            if (shouldSkipToken)
                SkipToken();

            errorHandler.RaiseCompilationError(error);
            token = Token.Unknown(TokenPosition);
            return false;
        }

        protected virtual void SkipToken()
        {
            while(!Source.CurrentCharacter.IsTokenTerminator())
                Source.MoveToNext();

            // @TODO: handle situation when invalid token is too long
        }
    }
}
