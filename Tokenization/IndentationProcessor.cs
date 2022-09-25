namespace Uncertainty.Tokenization;


public static class IndentationProcessor
{
    public static IEnumerable<Token> ProcessIndentation(IEnumerable<Token> inputStream)
    {
        var enumerator = inputStream.GetEnumerator();
        enumerator.MoveNext();
        Stack<IndentationLevel> indentations = new();
        indentations.Push(new IndentationLevel(0));

        IndentationType currentType = IndentationType.None;

        while (true)
        {
            if (enumerator.Current.Type == TokenType.EOF)
            {
                while (indentations.Count > 1)
                {
                    indentations.Pop();
                    yield return new Token(TokenType.IND_Outdent, null, null);
                }

                break;
            } else if (enumerator.Current.Type == TokenType.Newline)
            {
                Token t = enumerator.Current;
                enumerator.MoveNext();

                if (enumerator.Current.Type == TokenType.Whitespace)
                {
                    int level;
                    IndentationType type = GetIndentationType(enumerator.Current.Content!, out level);

                    if (type == IndentationType.Mixed || currentType != IndentationType.None && currentType != type)
                    {
                        throw new Exception("Mixed use of tabs and spaces for indentation");
                    }
                    else
                    {
                        currentType = type;
                    }

                    int topLevel = indentations.Peek().Depth;

                    if (level > topLevel)
                    {
                        indentations.Push(new IndentationLevel(level));
                        yield return new Token(TokenType.IND_Indent, null, null);
                    }
                    //else if (level == topLevel)
                    //{
                    //    yield return enumerator.Current;
                    //}
                    else if (level < topLevel)
                    {
                        while (level < indentations.Peek().Depth)
                        {
                            indentations.Pop();
                            yield return new Token(TokenType.IND_Outdent, null, null);
                        }
                        if (level != indentations.Peek().Depth)
                        {
                            throw new Exception($"Mismatched indentation: Expected indentation of {indentations.Peek().Depth} but got {level}");
                        }
                        if (level == 0)
                        {
                            currentType = IndentationType.None;
                        }
                    } else
                    {
                        yield return t;
                    }

                    enumerator.MoveNext();
                }
                else
                {   
                    if (indentations.Count > 1)
                    {
                        while (indentations.Count > 1)
                        {
                            indentations.Pop();
                            yield return new Token(TokenType.IND_Outdent, null, null);
                        }
                    } else
                    {
                        yield return t;
                    }
                    

                    currentType = IndentationType.None;
                    yield return enumerator.Current;
                    enumerator.MoveNext();
                }
            } 
            else if (enumerator.Current.Type == TokenType.Whitespace)
            {
                enumerator.MoveNext();
            }
            else
            {
                yield return enumerator.Current;
                enumerator.MoveNext();
            }
        }

        IndentationType GetIndentationType(string content, out int level)
        {
            level = 0;
            IndentationType output = IndentationType.None;

            while (level < content.Length)
            {
                if (content[level] == ' ')
                {
                    output = output is not (IndentationType.None or IndentationType.Spaces) ? IndentationType.Mixed : IndentationType.Spaces;
                } else if (content[level] == '\t')
                {
                    output = output is not (IndentationType.None or IndentationType.Tabs) ? IndentationType.Mixed : IndentationType.Tabs;
                } else
                {
                    throw new ArgumentException("Got unexpected character as whitespace.");
                }

                level++;
            }

            return output;
        }
        yield return enumerator.Current;
    }

    record struct IndentationLevel(int Depth);

    enum IndentationType
    {
        None,
        Mixed,
        Tabs,
        Spaces,
    }
}
