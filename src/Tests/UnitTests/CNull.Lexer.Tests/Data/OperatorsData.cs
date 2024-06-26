﻿using CNull.Common;
using CNull.Lexer.Constants;

namespace CNull.Lexer.Tests.Data
{
    internal class OperatorsData : TheoryData<string, bool, Token>
    {
        public OperatorsData()
        {
            foreach (var op in TokenHelpers.OperatorsToTokenTypes)
                Add($"{op.Key}something();", true, new Token(op.Value, Position.FirstCharacter));

            Add("+++", true, new Token(TokenType.PlusOperator, Position.FirstCharacter));
            Add("-=", true, new Token(TokenType.MinusOperator, Position.FirstCharacter));
            Add("'asacassaa", false, Token.Unknown(Position.FirstCharacter));
            Add("/=", true, new Token(TokenType.SlashOperator, Position.FirstCharacter));
            Add("!==>abcde", true, new Token(TokenType.NotEqualOperator, Position.FirstCharacter));
        }
    }

    internal class OperatorsLastCharacterData : TheoryData<string, char?>
    {
        public OperatorsLastCharacterData()
        {
            foreach (var op in TokenHelpers.OperatorsAndPunctors)
                Add($"{op}something();", 's');

            Add("+++", '+');
            Add("-=", '=');
            Add("&&", null);
            Add("&&&", '&');
            Add("!=/>abcde", '/');
        }
    }
}
