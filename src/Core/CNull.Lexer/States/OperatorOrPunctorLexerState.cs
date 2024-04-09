using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
using CNull.Lexer.ServicesContainers;

namespace CNull.Lexer.States
{
    /// <summary>
    /// Represents a state in which operator or punctor is built.
    /// </summary>
    public class OperatorOrPunctorLexerState(ILexerStateServicesContainer servicesContainer, ILexerState commentLexerState) : LexerState(servicesContainer)
    {
        private string _operator = string.Empty;

        public override bool TryBuildToken(out Token token)
        {
            if (!CurrentCharacter.IsOperatorCandidate())
                return TokenFailed(out token, new InvalidTokenStartCharacter(TokenPosition), false);

            _operator += CurrentCharacter;
            Source.MoveToNext();

            var doubleOperatorCandidate = _operator + CurrentCharacter;
            if (TokenHelpers.OperatorsAndPunctors.Contains(doubleOperatorCandidate))
            {
                Source.MoveToNext();
                token = new Token<string>(doubleOperatorCandidate, TokenType.OperatorOrPunctor, TokenPosition);
                return true;
            }

            if (doubleOperatorCandidate == "//")
                return commentLexerState.TryBuildToken(out token);

            if (!TokenHelpers.OperatorsAndPunctors.Contains(_operator)) 
                return TokenFailed(out token, new UnknownOperatorError(TokenPosition));

            token = new Token<string>(_operator, TokenType.OperatorOrPunctor, TokenPosition);
            return true;
        }
    }
}
