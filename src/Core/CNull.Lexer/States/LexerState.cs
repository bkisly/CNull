using CNull.Common;
using CNull.Common.Configuration;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// Base class for lexer states that interact with the source.
    /// </summary>
    /// <param name="source"></param>
    public abstract class LexerState(ICodeSource source, IErrorHandler errorHandler, ICompilerConfiguration configuration) : ILexerState
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
            var counter = 0;
            while (!Source.CurrentCharacter.IsTokenTerminator())
            {
                if (++counter > configuration.MaxTokenLength)
                    errorHandler.RaiseCompilationError(new InvalidTokenLengthError(TokenPosition, configuration.MaxTokenLength));

                Source.MoveToNext();
            }
        }
    }
}
