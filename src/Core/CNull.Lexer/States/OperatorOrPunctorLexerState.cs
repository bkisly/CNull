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

        public override Token BuildToken()
        {
            if (!CurrentCharacter.IsOperatorCandidate())
                return TokenFailed(new InvalidTokenStartCharacter(TokenPosition), false);

            _operator += CurrentCharacter;
            Source.MoveToNext();

            var doubleOperatorCandidate = _operator + CurrentCharacter;
            if (TokenHelpers.OperatorsAndPunctors.Contains(doubleOperatorCandidate))
            {
                Source.MoveToNext();
                return new Token(TokenHelpers.OperatorsToTokenTypes[doubleOperatorCandidate], TokenPosition);
            }

            if (doubleOperatorCandidate == "//")
                return commentLexerState.BuildToken();

            return !TokenHelpers.OperatorsAndPunctors.Contains(_operator) 
                ? TokenFailed(new UnknownOperatorError(TokenPosition)) 
                : new Token(TokenHelpers.OperatorsToTokenTypes[_operator], TokenPosition);
        }
    }
}
