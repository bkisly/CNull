using System.Text;
using CNull.Common.Extensions;
using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    public class CommentLexerState(ICodeSource source) : LexerState(source)
    {
        private readonly StringBuilder _builder = new();

        public override bool TryBuildToken(out Token token)
        {
            while (Source.CurrentCharacter.IsWhiteSpace() && !Source.IsCurrentCharacterNewLine)
                Source.MoveToNext();

            while (Source is { IsCurrentCharacterNewLine: false, CurrentCharacter: not null })
            {
                // @TODO: check for excessively long comments
                _builder.Append(Source.CurrentCharacter);
                Source.MoveToNext();
            }

            Source.MoveToNext();
            token = new Token<string>(_builder.ToString(), TokenType.Comment);
            return true;
        }
    }
}
