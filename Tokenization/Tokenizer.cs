using System.Collections;
using System.Text;

namespace Uncertainty.Tokenization;

public class Tokenizer
{
    public IEnumerable<char> Source { get; private set; }
    private IEnumerator<char> enumerator;

    private bool eof = false;

    private Dictionary<string, TokenType> keywords = new()
    {
        { "if", TokenType.KW_If },
        { "else", TokenType.KW_Else },
        { "break", TokenType.KW_Break },
        { "continue", TokenType.KW_Continue },
        { "def", TokenType.KW_Def },
        { "let", TokenType.KW_Let },
        { "var", TokenType.KW_Var },

    };

    int line = 1;
    int col = 1;

    public Tokenizer(IEnumerable<char> source)
    {
        Source = source;
        enumerator = source.GetEnumerator();
    }

    public IEnumerable<Token> Tokenize()
    {
        if (!eof && !MoveNext()) yield return Eof();

        // Handle BOM
        if (!eof && (enumerator.Current == '\uFFFE' || enumerator.Current == '\uFEFF'))
        {
            enumerator.MoveNext();
        }

        while (!eof)
        {
            switch (enumerator.Current)
            {
                case '\n' or '\r':
                    yield return ConsumeNewline();
                    break;
                case '(' or ')' or ':' or '%' or ',' or '$' or '_' or '.' or '+' or '-' or '=' or '>' or '<' or '*' or '/' or '[' or ']':
                    yield return ConsumeCharacter();
                    break;
                case '"':
                    yield return ConsumeString();
                    break;
                case ' ' or '\t':
                    yield return ConsumeWhitespace();
                    break;
                default:
                    if (char.IsDigit(enumerator.Current))
                    {
                        yield return ConsumeNumber();
                        break;
                    }

                    yield return ConsumeIdentifier();
                    break;
            }
        }

        yield return new Token(TokenType.EOF, null, null);
    }

    private Token ConsumeNewline()
    {
        Token t;
        
        if (enumerator.Current == '\r')
        {
            RequireMoveNext();
            if (enumerator.Current != '\n')
            {
                throw new Exception($"Unexpected character {enumerator.Current} at position {line}:{col + 1} when consuming newline: got CR (\\r) without corresponding LF (\\n)");
            }

            t = new Token(TokenType.Newline, "\r\n", new(new(line, col), new(line, col + 2)));
        } else
        {
            t = new Token(TokenType.Newline, "\n", new(new(line, col), new(line, col + 1)));
        }

        col = 1;
        line += 1;
        MoveNext();

        return t;
    }

    private Token ConsumeWhitespace()
    {
        StringBuilder chars = new();
        int index = 0;

        if (enumerator.Current is not (' ' or '\t'))
        {
            throw new Exception($"Unexpected character {enumerator.Current} at position {line}:{col + index} when consuming whitespace");
        }

        chars.Append(enumerator.Current);
        RequireMoveNext();
        index++;

        while (true)
        {
            if (enumerator.Current is not (' ' or '\t'))
            {
                break;
            }

            chars.Append(enumerator.Current);
            MoveNext();
            index++;
        }

        Token t = new Token(TokenType.Whitespace, chars.ToString(), new(new(line, col), new(line, col + index)));
        col += index;
        return t;
    }

    private Token ConsumeCharacter()
    {
        TokenType type = enumerator.Current switch
        {
            '(' => TokenType.LParen,
            ')' => TokenType.RParen,
            ':' => TokenType.Colon,
            '%' => TokenType.Percent,
            ',' => TokenType.Comma,
            '$' => TokenType.Dollar,
            '_' => TokenType.Underscore,
            '.' => TokenType.Period,
            '+' => TokenType.Plus,
            '-' => TokenType.Minus,
            '=' => TokenType.Equals,
            '>' => TokenType.Gt,
            '<' => TokenType.Lt,
            '[' => TokenType.LBrace,
            ']' => TokenType.RBrace,
            '*' => TokenType.Asterisk,
            '/' => TokenType.Slash,
            _ => throw new Exception($"Unexpected character {enumerator.Current} at position {line}:{col} when consuming character")
        };

        string c = enumerator.Current.ToString();
        MoveNext();
        col++;

        if (type == TokenType.Minus)
        {
            if (enumerator.Current == '>')
            {
                type = TokenType.TypeArrow;
                c += enumerator.Current.ToString();
                MoveNext();
                col++;
            }
        } else if (type == TokenType.Equals)
        {
            if (enumerator.Current == '=')
            {
                type = TokenType.DoubleEquals;
                c += enumerator.Current.ToString();
                MoveNext();
                col++;
            }  
        } else if (type == TokenType.Colon)
        {
            if (enumerator.Current == ':')
            {
                type = TokenType.DoubleColon;
                c += enumerator.Current.ToString();
                MoveNext();
                col++;
            }
        } else if (type == TokenType.LParen)
        {
            if (enumerator.Current == ')')
            {
                type = TokenType.UnitLiteral;
                c += enumerator.Current.ToString();
                MoveNext();
                col++;
            }
        }

        Token t = new Token(type, c,
            new(new(line, col), new(line, col + 1)));

        return t;
    }

    private Token ConsumeNumber()
    {
        StringBuilder chars = new();
        int index = 0;

        if (!char.IsDigit(enumerator.Current))
        {
            throw new Exception($"Unexpected character {enumerator.Current} at position {line}:{col + index} when consuming number literal");
        }

        bool lastWasUnderscore = false;
        while (true)
        {
            chars.Append(enumerator.Current);
            if (!MoveNext()) break;
            index++;

            if (!char.IsDigit(enumerator.Current) && enumerator.Current != '_')
            {
                break;
            }
        }

        if (lastWasUnderscore)
        {
            throw new Exception($"Unexpected character _ at position {line}:{col + index} when consuming number literal: Numbers cannot end with an underscore.");
        }

        Token t = new Token(TokenType.IntLiteral, chars.ToString(), new(new(line, col), new(line, col + index)));
        col += index;
        return t;
    }

    private Token ConsumeString()
    {
        StringBuilder chars = new();
        int index = 0;

        if (enumerator.Current != '"')
        {
            throw new Exception($"Unexpected character {enumerator.Current} at position {line}:{col + index} when consuming string");
        }

        chars.Append(enumerator.Current);
        RequireMoveNext();
        index++;

        while (true)
        {
            if (enumerator.Current == '"')
            {
                break;
            }

            if (enumerator.Current == '\\')
            {
                throw new NotSupportedException($"Unexpected character {enumerator.Current} at position {line}:{col + index} when consuming string: Escape sequences are not currently supported.");
            }

            if (enumerator.Current is '\n')
            {
                throw new Exception($"Unexpected character (\\n) at position {line}:{col + index} when consuming string: Strings cannot contain newlines.");
            }

            chars.Append(enumerator.Current);
            RequireMoveNext();
            index++;
        }

        if (enumerator.Current != '"')
        {
            throw new Exception($"Unexpected character {enumerator.Current} at position {line}:{col + index} when consuming string");
        }

        chars.Append(enumerator.Current);
        MoveNext();
        index++;

        Token t = new Token(TokenType.StringLiteral, chars.ToString(), new(new(line, col), new(line, col + index)));
        col += index;
        return t; 
    }

    private Token ConsumeIdentifier()
    {
        StringBuilder chars = new();
        int index = 0;

        if (!char.IsLetter(enumerator.Current))
        {
            throw new Exception($"Unexpected character {enumerator.Current} at position {line}:{col + index} when consuming identifier");
        }

        while (true)
        {
            chars.Append(enumerator.Current);
            if (!MoveNext()) break;
            index++;

            if (!char.IsLetterOrDigit(enumerator.Current) && enumerator.Current is not '_')
            {
                break;
            }
        }

        string ident = chars.ToString();
        TokenType type = keywords.ContainsKey(ident) ? keywords[ident] : TokenType.Identifier;

        Token t = new Token(type, chars.ToString(), new(new(line, col), new(line, col + index)));
        col += index;

        return t;
    }

    private Token Eof()
    {
        return new Token(TokenType.EOF, null, null);
    }

    private void RequireMoveNext()
    {
        if (!enumerator.MoveNext())
        {
            throw new Exception("Unexpected end of input");
        }
    }

    private bool MoveNext()
    {
        if (!enumerator.MoveNext())
        {
            eof = true;
            return false;
        }
        else return true;
    }
}