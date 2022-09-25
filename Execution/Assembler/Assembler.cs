namespace Uncertainty.Execution.Assembler;

public class AssemblerTokenizer
{
    string input;
    int index;

    public AssemblerTokenizer(string input)
    {
        this.input = input;
        this.index = 0;
    }

    public List<AssemblerToken> Tokenize()
    {
        List<AssemblerToken> tokens = new();
        while (index < input.Length)
        {
            char c = input[index];
            switch (c)
            {
                case '\n' or ';':
                    tokens.Add(new AssemblerToken(input[index].ToString(), AssemblerTokenType.Break));
                    index++;
                    break;

                case ',':
                    tokens.Add(new AssemblerToken(input[index].ToString(), AssemblerTokenType.Comma));
                    index++;
                    break;

                case '(':
                    tokens.Add(new AssemblerToken(input[index].ToString(), AssemblerTokenType.LeftDelimiter));
                    index++;
                    break;

                case ')':
                    tokens.Add(new AssemblerToken(input[index].ToString(), AssemblerTokenType.RightDelimiter));
                    index++;
                    break;

                case ':':
                    {
                        int startPoint = index;
                        index++;
                        while (index < input.Length)
                        {
                            if (char.IsLetter(input[index])) { index++; continue; }
                            else { break; }
                        }
                        tokens.Add(new AssemblerToken(input.Substring(startPoint + 1, index - startPoint - 1), AssemblerTokenType.LabelReference));
                        break;
                    }

                case '/':
                    index++;
                    if (input[index] == '*')
                    {
                        while (index < input.Length)
                        {
                            if (input[index] == '*')
                            {
                                index++;
                                if (input[index] == '/')
                                {
                                    index++; break;
                                }
                            }
                            else { index++; continue; }
                        }
                    } else if (input[index] == '/')
                    {
                        while (index < input.Length)
                        {
                            if (input[index] == '\n') break;
                            else { index++; continue; }
                        }
                    }
                    break;

                case '%':
                    {
                        int startPoint = index;
                        index++;
                        while (index < input.Length)
                        {
                            if (char.IsLetter(input[index])) { index++; continue; }
                            else { break; }
                        }
                        tokens.Add(new AssemblerToken(input.Substring(startPoint + 1, index - startPoint - 1), AssemblerTokenType.ConstReference));
                        break;
                    }

                default:
                    if (char.IsLetter(c) || c == '.')
                    {
                        int startPoint = index;
                        bool isLabel = false;
                        bool isKeyword = false;
                        while (index < input.Length)
                        {
                            if (char.IsLetter(input[index]) || char.IsDigit(input[index])) { index++; continue; }
                            else if (input[index] == '.') { index++; isKeyword = true; continue; }
                            else if (input[index] == ':') { index++; isLabel = true; break; }
                            else { break; }
                        }
                        if (isKeyword && isLabel)
                        {
                            throw new Exception("Identifier contains both : and ., meaning it is both a label and a keyword.");
                        }
                        if (isKeyword)
                        {
                            tokens.Add(new AssemblerToken(input.Substring(startPoint, index - startPoint), AssemblerTokenType.Keyword));
                            break;
                        }
                        if (isLabel)
                        {
                            tokens.Add(new AssemblerToken(input.Substring(startPoint, index - startPoint - 1), AssemblerTokenType.LabelDefinition));
                            break;
                        } else
                        {
                            tokens.Add(new AssemblerToken(input.Substring(startPoint, index - startPoint), AssemblerTokenType.Instruction));
                            break;
                        }
                    } else if (char.IsDigit(c))
                    {
                        // Hex
                        if (input[index] == '0' && input[index + 1] == 'x')
                        {
                            index += 2;
                            int sp = index;
                            while (index < input.Length)
                            {
                                if (char.IsDigit(input[index]) ||
                                        input[index] is 'a' or 'A' or 'b' or 'B' or 'c' or 'C' 
                                        or 'd' or 'D' or 'e' or 'E' or 'f' or 'F') { index++; continue; }
                                else { break; }
                            }
                            tokens.Add(new AssemblerToken(input.Substring(sp, index - sp), AssemblerTokenType.HexLiteral));
                            break;
                        } else
                        {
                            // Number or Percent
                            int startPoint = index;
                            while (index < input.Length)
                            {
                                if (char.IsDigit(input[index])) { index++; continue; }
                                else { break; }
                            }
                            tokens.Add(new AssemblerToken(input.Substring(startPoint, index - startPoint), AssemblerTokenType.IntegerLiteral));
                        }
                    } else { index++; }
                    break;
            }
        }

        return tokens;
    }
}

public class AssemblerParser
{
    List<AssemblerToken> tokens;
    List<byte> bytes;
    Dictionary<string, int> labels;
    Dictionary<string, byte[]> constants;
    List<(string, int)> labelReferences;
    int index;

    public AssemblerParser(List<AssemblerToken> tokens)
    {
        this.tokens = tokens;
        bytes = new();
        labels = new ();
        constants = new();
        constants.Add("word", new byte[4]);
        labelReferences = new List<(string, int)>();
        index = 0;
    }

    Dictionary<string, AssemblerInstruction> instructions = new()
    {
        { "noop",       new(new byte[] { (byte)Operations.Noop },      new int[] { }) },
        { "halt",       new(new byte[] { (byte)Operations.Halt },      new int[] { }) },
        { "debug",      new(new byte[] { (byte)Operations.Debug },     new int[] { }) },
        { "push",       new(new byte[] { (byte)Operations.Push },      new int[] { 4 }) },
        { "pop",        new(new byte[] { (byte)Operations.Pop },       new int[] { }) },
        { "dup",        new(new byte[] { (byte)Operations.Dup },       new int[] { }) },
        { "swap",       new(new byte[] { (byte)Operations.Swap },      new int[] { }) },
        { "rrotate",    new(new byte[] { (byte)Operations.RRotate },   new int[] { }) },
        { "lrotate",    new(new byte[] { (byte)Operations.LRotate },   new int[] { }) },
        { "stor4",      new(new byte[] { (byte)Operations.Stor4 },     new int[] { 4 }) },
        { "sstor4",     new(new byte[] { (byte)Operations.SStor4 },    new int[] { }) },
        { "load4",      new(new byte[] { (byte)Operations.Load4 },     new int[] { 4 }) },
        { "sload4",     new(new byte[] { (byte)Operations.SLoad4 },    new int[] { }) },
        { "allocloc",   new(new byte[] { (byte)Operations.Allocloc },  new int[] { 4 }) },
        { "storloc4",   new(new byte[] { (byte)Operations.StorLoc4 },  new int[] { 2 }) },
        { "loadloc4",   new(new byte[] { (byte)Operations.LoadLoc4 },  new int[] { 2 }) },
        { "loadarg4",   new(new byte[] { (byte)Operations.LoadArg4 },  new int[] { 2 }) },
        { "add",        new(new byte[] { (byte)Operations.Add },       new int[] { }) },
        { "sub",        new(new byte[] { (byte)Operations.Sub },       new int[] { }) },
        { "jmp",        new(new byte[] { (byte)Operations.Jmp },       new int[] { 4 }) },
        { "breq",       new(new byte[] { (byte)Operations.BrEq },      new int[] { 4, 4 }) },
        { "brneq",      new(new byte[] { (byte)Operations.BrNeq },     new int[] { 4, 4 }) },
        { "brgt",       new(new byte[] { (byte)Operations.BrGt },      new int[] { 4, 4 }) },
        { "brgteq",     new(new byte[] { (byte)Operations.BrGteq },    new int[] { 4, 4 }) },
        { "brlt",       new(new byte[] { (byte)Operations.BrLt },      new int[] { 4, 4 }) },
        { "brlteq",     new(new byte[] { (byte)Operations.BrLteq },    new int[] { 4, 4 }) },
        { "brrand",     new(new byte[] { (byte)Operations.BrRand },    new int[] { 1, 4 }) },
        { "call",       new(new byte[] { (byte)Operations.Call },      new int[] { 4 }) },
        { "ret",        new(new byte[] { (byte)Operations.Ret },       new int[] { }) },
        { "ret4",       new(new byte[] { (byte)Operations.Ret4 },      new int[] { }) },
    };

    public byte[] Parse()
    {
        // Preprocess constants
        while (index < tokens.Count)
        {
            var t = tokens[index];
            switch (t.TokenType)
            {
                case AssemblerTokenType.Keyword:
                    if (t.Content == ".data")
                    {
                        while (tokens[index].TokenType != AssemblerTokenType.Break) { index++; }
                        break;
                    }
                    else if (t.Content == ".const")
                    {
                        index++;

                        // Name
                        if (tokens[index].TokenType != AssemblerTokenType.Instruction)
                        {
                            throw new Exception($"Invalid identifier for constant - got {t}");
                        }
                        string name = tokens[index].Content;
                        index++;

                        // Value
                        if (tokens[index].TokenType is not (AssemblerTokenType.IntegerLiteral or AssemblerTokenType.HexLiteral))
                        {
                            throw new Exception($"Cannot use {t} as constant value");
                        }
                        var val = GetTokenValue(tokens[index]);
                        index++;

                        byte[] dat;

                        // Optional Width
                        if (tokens[index].TokenType is 
                            (AssemblerTokenType.IntegerLiteral or AssemblerTokenType.HexLiteral))
                        {
                            var width = GetTokenValue(tokens[index]);
                            dat = Widen(GetSmallestValue(val), (int)width);
                            index++;
                        } 
                        else 
                        {
                            dat = GetSmallestValue(val);
                        }

                        if (constants.ContainsKey(name))
                        {
                            throw new Exception($"Duplicate constant declaration with name {name}");
                        }
                        constants.Add(name, dat);

                        if (tokens[index].TokenType == AssemblerTokenType.Break)
                        {
                            index++;
                            break;
                        } 
                        else
                        {
                            throw new Exception($"Unexpected token {t} in constant declaration {name}");
                        }
                    }
                    else
                    {
                        throw new Exception($"Unknown keyword {t.Content}");
                    }

                default:
                    index++;
                    break;
            }
        }

        index = 0;
        while (index < tokens.Count)
        {
            var t = tokens[index];
            switch (t.TokenType)
            {
                case AssemblerTokenType.LabelDefinition:
                    index++;
                    if (labels.ContainsKey(t.Content))
                    {
                        throw new Exception($"Duplicate label declaration: {t.Content}");
                    }
                    labels.Add(t.Content, bytes.Count);
                    break;

                case AssemblerTokenType.Instruction:
                    if (instructions.TryGetValue(t.Content, out var inst))
                    {
                        bytes.AddRange(inst.Code);
                        index++;
                        for (int i = 0; i < inst.Params.Length; i++)
                        {
                            bool gotComma = false;
                            if (i > 0)
                            {
                                if (gotComma && tokens[index].TokenType == AssemblerTokenType.Comma)
                                {
                                    throw new Exception("Got duplicate commas");
                                }
                                if (tokens[index].TokenType == AssemblerTokenType.Comma)
                                {
                                    gotComma = true;
                                    index++;
                                }
                            }

                            var newTok = tokens[index++];
                            byte[] value = ResizeTokenToValue(newTok, inst.Params[i]);
                            if (value.Length != inst.Params[i])
                            {
                                throw new Exception($"Couldn't convert {newTok} to value of size {inst.Params[i]}");
                            }
                            bytes.AddRange(value);
                        }

                        if (index + 1 < tokens.Count && tokens[index++].TokenType != AssemblerTokenType.Break)
                        {
                            throw new Exception($"Expected end of parameter list");
                        }
                    }
                    else
                    {
                        throw new Exception($"Unknown instruction: {t.Content}");
                    }
                    break;

                case AssemblerTokenType.Keyword:
                    if (t.Content == ".data")
                    {
                        index++;
                        while (tokens[index].TokenType != AssemblerTokenType.Break)
                        {
                            var smallest = GetSmallestTokenValue(tokens[index]);
                            bytes.AddRange(smallest);
                            index++;
                            if (tokens[index].TokenType == AssemblerTokenType.Comma) { index++; }
                        }
                        index++;
                        break;
                        //if (tokens[index].TokenType == AssemblerTokenType.LeftDelimiter)
                        //{
                        //    index++;
                        //    while (tokens[index].TokenType != AssemblerTokenType.RightDelimiter)
                        //    {
                        //        bytes.AddRange(GetSmallestTokenValue(tokens[index]));
                        //        index++;
                        //        if (tokens[index].TokenType == AssemblerTokenType.Comma) { index++; }
                        //    }
                        //    index++;
                        //}
                        //else
                        //{
                        //    bytes.AddRange(GetSmallestTokenValue(tokens[index]));
                        //    index++;
                        //}
                        //break;
                    }
                    else if (t.Content == ".const")
                    {
                        while (tokens[index].TokenType != AssemblerTokenType.Break) { index++; }
                        break;
                    }
                    else
                    {
                        throw new Exception($"Unknown keyword {t.Content}");
                    }

                default:
                    index++;
                    break;
            }
        }

        int offset = 0;

        if (labels.ContainsKey("start"))
        {
            offset = 5;
        }

        foreach (var (label, loc) in labelReferences)
        {
            if (!labels.TryGetValue(label, out var val))
            {
                throw new Exception($"Reference to undeclared label: {label}");
            }

            byte[] value = BitConverter.GetBytes(val + offset);

            for (int i = 0; i < value.Length; i++)
            {
                bytes[i + loc] = value[i];
            }
        }

        if (labels.TryGetValue("start", out int pos))
        {
            var l = new List<byte>();
            l.Add((byte)Operations.Jmp);
            l.AddRange(BitConverter.GetBytes(pos + offset));
            l.AddRange(bytes);
            return l.ToArray();
        }
        return bytes.ToArray();
    }

    ulong GetTokenValue(AssemblerToken t)
    {
        switch (t.TokenType)
        {
            case AssemblerTokenType.IntegerLiteral:
                return ulong.Parse(t.Content);

            case AssemblerTokenType.HexLiteral:
                if (t.Content.Length % 2 != 0) 
                    throw new Exception($"Hexadecimal literals must have an even number of characters (got {t.Content.Length})");
                return ulong.Parse(t.Content, System.Globalization.NumberStyles.HexNumber);

            case AssemblerTokenType.LabelReference:
                return 0;

            default:
                throw new Exception($"Cannot get numeric value for token {t}");
        }
    }

    /// <summary>
    /// !!!! ALSO ADDRESSES LABEL REFERENCES !!!!
    /// </summary>
    byte[] GetSmallestTokenValue(AssemblerToken t)
    {
        ulong val;
        switch (t.TokenType)
        {
            case AssemblerTokenType.IntegerLiteral:
                val = GetTokenValue(t);
                return GetSmallestValue(val);

            case AssemblerTokenType.HexLiteral:
                val = GetTokenValue(t);
                return Widen(GetSmallestValue(val), t.Content.Length / 2);

            case AssemblerTokenType.ConstReference:
                return constants[t.Content];

            case AssemblerTokenType.LabelReference:
                labelReferences.Add((t.Content, bytes.Count));
                return new byte[] { 0, 0, 0, 0 };

            default:
                throw new Exception($"Cannot get numeric value for token {t}");
        }
    }

    byte[] GetSmallestValue(ulong val)
    {
        if (val <= byte.MaxValue)
        {
            return new byte[] { (byte)val };
        }
        else if (val <= ushort.MaxValue)
        {
            return BitConverter.GetBytes((ushort)val);
        }
        else if (val <= uint.MaxValue)
        {
            return BitConverter.GetBytes((uint)val);
        }
        else return BitConverter.GetBytes(val);
    }

    byte[] Widen(byte[] value, int size)
    {
        byte[] widened = new byte[size];
        //Array.Copy(value, 0, widened, size - value.Length, value.Length);
        Array.Copy(value, widened, value.Length);
        return widened;
    }

    bool IsNumericToken(AssemblerToken t) =>
        t.TokenType is AssemblerTokenType.LabelReference or AssemblerTokenType.IntegerLiteral or AssemblerTokenType.HexLiteral or AssemblerTokenType.ConstReference;

    byte[] ResizeTokenToValue(AssemblerToken t, int byteSize)
    {
        if (byteSize is not (1 or 2 or 4 or 8))
        {
            throw new ArgumentException("Argument must be 1, 2, 4 or 8", nameof(byteSize));
        }
        switch (t.TokenType)
        {
            case AssemblerTokenType.IntegerLiteral:
            case AssemblerTokenType.HexLiteral:
            case AssemblerTokenType.LabelReference:
            case AssemblerTokenType.ConstReference:
                var smallest = GetSmallestTokenValue(t);
                if (smallest.Length > byteSize)
                {
                    throw new Exception($"Cannot use {t} (size {smallest.Length}) in context where length {byteSize} is required.");
                }

                return Widen(smallest, byteSize);

            default:
                throw new Exception($"Cannot get numeric value for token {t}");
        }
    }
}

struct AssemblerInstruction
{
    public byte[] Code { get; init; }
    public int[] Params { get; init; }

    public AssemblerInstruction(byte[] code, int[] parameters)
    {
        Code = code;
        Params = parameters;
    }
}

public record class AssemblerToken
{
    public string Content { get; init; }
    public AssemblerTokenType TokenType { get; init; }

    public AssemblerToken(string content, AssemblerTokenType tokenType)
    {
        Content = content;
        TokenType = tokenType;
    }

    public override string ToString()
    {
        return $"(\"{Content.Replace("\n", "\\n")}\", {TokenType})";
    }
}

public enum AssemblerTokenType
{
    Instruction,
    Keyword,
    LabelDefinition,
    LabelReference,
    ConstReference,
    IntegerLiteral,
    HexLiteral,
    Comma,
    LeftDelimiter,
    RightDelimiter,
    Break,
}
