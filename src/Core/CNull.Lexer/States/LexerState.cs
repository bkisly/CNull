using CNull.Common;
using CNull.Common.Configuration;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
using CNull.Lexer.ServicesContainers;
using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// Base class for lexer states that interact with the source.
    /// </summary>
    public abstract class LexerState : ILexerState
    {
        protected ICodeSource Source;
        protected IErrorHandler ErrorHandler;
        protected ICompilerConfiguration Configuration;

        protected char? CurrentCharacter => Source.CurrentCharacter;
        protected readonly Position TokenPosition;

        protected LexerState(ILexerStateServicesContainer lexerStateServices)
        {
            Source = lexerStateServices.CodeSource;
            ErrorHandler = lexerStateServices.ErrorHandler;
            Configuration = lexerStateServices.CompilerConfiguration;
            TokenPosition = Source.Position;
        }

        public abstract Token BuildToken();

        protected Token TokenFailed(ICompilationError error, bool shouldSkipToken = true)
        {
            if (shouldSkipToken)
                SkipToken();

            ErrorHandler.RaiseCompilationError(error);
            return Token.Unknown(TokenPosition);
        }

        protected virtual void SkipToken()
        {
            var counter = 0;
            while (!Source.CurrentCharacter.IsTokenTerminator())
            {
                if (++counter > Configuration.MaxTokenLength)
                    ErrorHandler.RaiseCompilationError(new InvalidTokenLengthError(TokenPosition, Configuration.MaxTokenLength));

                Source.MoveToNext();
            }
        }
    }
}
