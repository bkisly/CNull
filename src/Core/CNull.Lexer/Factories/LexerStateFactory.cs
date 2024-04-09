using CNull.Lexer.Constants;
using CNull.Lexer.ServicesContainers;
using CNull.Lexer.States;

namespace CNull.Lexer.Factories
{
    public class LexerStateFactory(ILexerStateServicesContainer servicesContainer) 
        : ILexerStateFactory
    {
        private readonly Dictionary<Func<char, bool>, Func<ILexerState>> _predicateToStateFactory = new()
        {
            { c => char.IsLetter(c) || c == '_', () => new IdentifierOrKeywordLexerState(servicesContainer) },
            { char.IsAsciiDigit, () => new NumericLexerState(servicesContainer) },
            { c => c.IsOperatorCandidate(), () => new OperatorOrPunctorLexerState(servicesContainer, new CommentLexerState(servicesContainer)) },
            { c => c == '"', () => new StringLiteralLexerState(servicesContainer) },
            { c => c == '\'', () => new CharLiteralLexerState(servicesContainer) }
        };

        public ILexerState? Create(char firstCharacter)
        {
            return _predicateToStateFactory.Where(p => p.Key(firstCharacter))
                .Select(p => p.Value)
                .FirstOrDefault()?
                .Invoke();
        }
    }
}
