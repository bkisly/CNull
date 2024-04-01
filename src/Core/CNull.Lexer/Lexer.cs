using CNull.ErrorHandler;
using CNull.Lexer.Constants;
using CNull.Lexer.States;
using CNull.Source;

namespace CNull.Lexer
{
    public class Lexer : ILexer
    {
        private readonly ICodeSource _source;
        private readonly IErrorHandler _errorHandler;
        private ILexerState? _state;

        public Token? LastToken { get; private set; }

        public Lexer(ICodeSource source, IErrorHandler errorHandler)
        {
            _source = source;
            _errorHandler = errorHandler;

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
                return new Token(TokenType.End);

            if (!_state.TryBuildToken(out var token))
                throw new NotImplementedException("Implement an error when invalid token has been detected.");

            return token;
        }

        private void SkipWhitespace()
        {
            if (_source.CurrentCharacter == null)
                return;

            // @TODO: verify whitespace limit
            while(char.IsWhiteSpace(_source.CurrentCharacter.Value))
                _source.MoveToNext();
        }

        private ILexerState? GetNextState()
        {
            if(_source.CurrentCharacter == null)
                return null;

            throw new NotImplementedException();
        }
    }
}
