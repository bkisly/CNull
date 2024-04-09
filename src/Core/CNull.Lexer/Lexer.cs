using CNull.Common.Configuration;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
using CNull.Lexer.Factories;
using CNull.Lexer.ServicesContainers;
using CNull.Lexer.States;
using CNull.Source;

namespace CNull.Lexer
{
    public class Lexer : ILexer
    {
        private readonly ICodeSource _source;
        private readonly ILexerStateFactory _stateFactory;
        private readonly IErrorHandler _errorHandler;
        private readonly ICompilerConfiguration _configuration;
        private ILexerState? _state;

        public Token? LastToken { get; private set; }

        public Lexer(ILexerStateServicesContainer servicesContainer, ILexerStateFactory stateFactory)
        {
            _source = servicesContainer.CodeSource;
            _stateFactory = stateFactory;
            _errorHandler = servicesContainer.ErrorHandler;
            _configuration = servicesContainer.CompilerConfiguration;
            _source.SourceInitialized += Source_SourceInitialized;
        }

        private void Source_SourceInitialized(object? sender, EventArgs e)
        {
            LastToken = null;
            _state = null;
            _source.MoveToNext();
        }

        public Token GetNextToken()
        {
            SkipWhitespace();
            _state = GetNextState();

            if (_state == null)
                return CreateToken(new Token(TokenType.End, _source.Position));

            _state.TryBuildToken(out var token);
            return CreateToken(token);
        }

        private void SkipWhitespace()
        {
            if (_source.CurrentCharacter == null)
                return;

            var lengthCounter = 0;
            while (char.IsWhiteSpace(_source.CurrentCharacter.Value))
            {
                if (++lengthCounter > _configuration.MaxWhitespaceLength)
                {
                    _errorHandler.RaiseCompilationError(new InvalidTokenLengthError(_source.Position, _configuration.MaxWhitespaceLength));
                    return;
                }

                _source.MoveToNext();
            }
        }

        private Token CreateToken(Token token)
        {
            LastToken = token;
            return token;
        }

        private ILexerState? GetNextState()
        {
            return _source.CurrentCharacter == null ? null : _stateFactory.Create(_source.CurrentCharacter.Value);
        }
    }
}
