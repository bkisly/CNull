using CNull.Lexer.Constants;
using CNull.Lexer.States;
using CNull.Source;

namespace CNull.Lexer.Factories
{
    public class LexerStateFactory(ICodeSource source) : ILexerStateFactory
    {
        private readonly Dictionary<Func<char, bool>, ILexerState> _predicateToState = new()
        {
            { c => char.IsLetter(c) || c == '_', new IdentifierOrKeywordLexerState(source) },
            { char.IsAsciiDigit, new NumericLexerState(source) },
            { c => c.IsOperatorCandidate(), new OperatorOrPunctorLexerState(source, new CommentLexerState(source)) },
            { c => c == '"', new StringLiteralLexerState(source) },
            { c => c == '\'', new CharLiteralLexerState(source) }
        };

        public ILexerState? Create(char firstCharacter)
        {
            return _predicateToState.Where(p => p.Key(firstCharacter))
                .Select(p => p.Value)
                .FirstOrDefault();
        }
    }
}
