using CNull.Lexer.States;

namespace CNull.Lexer.Factories
{
    /// <summary>
    /// Creates new instances of lexer states based on the given character.
    /// </summary>
    public interface ILexerStateFactory
    {
        /// <summary>
        /// Creates a new instance of lexer state, based on the given first character.
        /// </summary>
        /// <param name="firstCharacter">First character, which marks the new state.</param>
        /// <returns></returns>
        public ILexerState? Create(char firstCharacter);
    }
}
