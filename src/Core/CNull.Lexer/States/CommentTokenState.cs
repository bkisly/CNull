using CNull.Source;

namespace CNull.Lexer.States
{
    public class CommentLexerState(ICodeSource source) : LexerState(source)
    {
        public override bool TryBuildToken(out Token token)
        {
            throw new NotImplementedException();
        }
    }
}
