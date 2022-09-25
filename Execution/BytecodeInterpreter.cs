namespace Uncertainty.Execution;

public class BytecodeInterpreter
{
    byte[] memory;
    int ip;
    int sp;
    int fp;
    int sfsize;
    bool running;
    int stfdepth = 0;
    Random r = new Random();

    public BytecodeInterpreter(byte[] instructions, int memSizeBytes)
    {
        ip = 0;
        sp = memSizeBytes - 1;
        fp = memSizeBytes - 1;
        sfsize = 0;
        running = true;
        memory = new byte[memSizeBytes];
        Array.Copy(instructions, 0, memory, 0, instructions.Length);
    }

    /// <summary>
    /// Read a single instruction from the current pointer and execute it
    /// </summary>
    public void Execute()
    {
        var fn = ((byte x, int i) =>
        {
            //return $"{(x.Length * 4 - i):X2}: {string.Join(" ", x.Select(x => $"{x:X2}"))}";
            if (i + 1 == memory.Length - sp)
            {
                return $"\x1B[32m{x:X2}\x1B[0m";
            }
            else if (i + 1 == memory.Length - fp)
            {
                return $"\x1B[31m{x:X2}\x1B[0m";
            }
            else
            {
                return $"{x:X2}";
            }
        });

        //Console.WriteLine($"{string.Join(" ", memory.Reverse().Take(8 * 10).Select((x, i) => fn(x, i)))}");
        Console.WriteLine((Operations)memory[ip]);
        //Console.WriteLine();
        switch ((Operations)memory[ip])
        {
            case Operations.Noop:
                {
                    ip++;
                    break;
                }


            case Operations.Halt:
                {
                    running = false;
                    ip++;
                    break;
                }


            case Operations.Push:
                {
                    ip++;
                    int val = ReadInt32FromBytes(ref memory, ref ip);
                    Push(val);
                    break;
                }
                

            case Operations.Pop:
                {
                    Console.WriteLine(Pop());
                    ip++;
                    break;
                }


            case Operations.Dup:
                {
                    int val = Pop();
                    Push(val);
                    Push(val);
                    ip++;
                    break;
                }

            case Operations.Stor4:
                {
                    ip++;
                    int value = Pop();
                    int addr = ReadInt32FromBytes(ref memory, ref ip);
                    WriteInt32FromBytes(ref memory, ref addr, value);
                    break;
                }
                

            case Operations.SStor4:
                {
                    ip++;
                    int addr = Pop();
                    int value = Pop();
                    WriteInt32FromBytes(ref memory, ref addr, value);
                    break;
                }

            case Operations.Load4:
                {
                    ip++;
                    int addr = ReadInt32FromBytes(ref memory, ref ip);
                    int num = ReadInt32FromBytes(ref memory, ref addr);
                    Push(num);
                    break;
                }

            case Operations.SLoad4:
                {
                    ip++;
                    int addr = Pop();
                    int num = ReadInt32FromBytes(ref memory, ref addr);
                    Push(num);
                    break;
                }

            case Operations.Allocloc:
                {
                    ip++;
                    int count = ReadInt32FromBytes(ref memory, ref ip);
                    sp -= count;
                    sfsize += count;
                    break;
                }

            case Operations.Call:
                {
                    ip++;
                    int addr = ReadInt32FromBytes(ref memory, ref ip);
                    // Push return address
                    Push(ip);

                    // Push frame pointer
                    Push(fp);

                    // Push current stack frame size
                    Push(sfsize + 4);

                    // Jump to address
                    stfdepth++;
                    //Console.WriteLine($"Calling from {ip:X4}; Current depth: {stfdepth}; \nsp:{sp:X4}\nip:{ip:X4}\nfp:{fp}\nsfsize:{sfsize}");
                    ip = addr;
                    sfsize = 0;
                    fp = sp;

                    break;
                }

            case Operations.Ret:
                {
                    sp = fp;
                    // Get current stack frame size
                    int frameSize = Pop();

                    // Get frame pointer
                    int framePointer = Pop();

                    // Get return address
                    int returnAddress = Pop();

                    stfdepth--;
                    //Console.WriteLine($"Returning to {ip:X4}; Current depth: {stfdepth}; \nsp:{sp:X4}\nip:{ip:X4}\nfp:{fp}\nsfsize:{frameSize}  \nra:{returnAddress}");

                    // Jump to address
                    ip = returnAddress;
                    sfsize = frameSize;
                    fp = framePointer;
                    


                    break;
                }

            case Operations.Ret4:
                {
                    sp = fp - 4;
                    // Get return element
                    int returnElem = Pop();

                    // Get current stack frame size
                    int frameSize = Pop();

                    // Get frame pointer
                    int framePointer = Pop();

                    // Get return address
                    int returnAddress = Pop();

                    // Jump to address
                    ip = returnAddress;
                    sfsize = frameSize;
                    fp = framePointer;

                    stfdepth--;
                    //Console.WriteLine($"Returning to {ip:X4}; Current depth: {stfdepth}; \nsp:{sp:X4}\nip:{ip:X4}\nfp:{fp}\nsfsize:{frameSize}");

                    // Push return element
                    Push(returnElem);

                    break;
                }

            case Operations.LoadArg4:
                {
                    ip++;
                    int index = ReadInt16FromBytes(ref memory, ref ip);
                    // 0x00 var <-fp
                    // 0x01 size
                    // 0x02 size
                    // 0x03 size
                    // 0x04 size
                    // 0x05 fp
                    // 0x06 fp
                    // 0x07 fp
                    // 0x08 fp
                    // 0x09 ret
                    // 0x0A ret
                    // 0x0B ret
                    // 0x0C ret
                    // 0x0D arg0
                    // 0x0E arg0
                    // 0x0F arg0
                    // 0x10 arg0
                    // 0x11 arg1 0x02
                    // 0x12 arg1 0x00
                    // 0x13 arg1 0x00
                    // 0x14 arg1 0x00
                    int p = fp + 12 + (index * 4);
                    Push(ReadInt32FromStack(ref memory, ref p));
                    break;
                }

            case Operations.LoadLoc4:
                {
                    ip++;
                    int index = ReadInt16FromBytes(ref memory, ref ip);
                    int p = fp + index * 4;
                    Push(ReadInt32FromBytes(ref memory, ref p));
                    break;
                }

            case Operations.StorLoc4:
                {
                    ip++;
                    int index = ReadInt16FromBytes(ref memory, ref ip);
                    int p = fp + index * 4;
                    int value = Pop();
                    WriteInt32FromBytes(ref memory, ref p, value);
                    break;
                }

            case Operations.Add:
                {
                    Push(Pop() + Pop());
                    ip++;
                    break;
                }

            case Operations.Sub:
                {
                    int b = Pop();
                    int a = Pop();
                    Push(a - b);
                    ip++;
                    break;
                }

            case Operations.Swap:
                {
                    ip++;
                    int b = Pop();
                    int a = Pop();
                    Push(b);
                    Push(a);
                    break;
                }

            case Operations.LRotate:
                {
                    ip++;
                    int c = Pop();
                    int b = Pop();
                    int a = Pop();
                    Push(b);
                    Push(c);
                    Push(a);
                    break;
                }

            case Operations.RRotate:
                {
                    ip++;
                    int c = Pop();
                    int b = Pop();
                    int a = Pop();
                    Push(c);
                    Push(a);
                    Push(b);
                    break;
                }

            case Operations.Jmp:
                {
                    ip++;
                    int addr = ReadInt32FromBytes(ref memory, ref ip);
                    ip = addr;
                    break;
                }

            case Operations.BrEq:
                {
                    ip++;
                    int condition = ReadInt32FromBytes(ref memory, ref ip);
                    int addr = ReadInt32FromBytes(ref memory, ref ip);
                    int val = Pop();
                    if (val == condition)
                    {
                        ip = addr;
                        break;
                    }
                    break;
                }

            case Operations.BrNeq:
                {
                    ip++;
                    int condition = ReadInt32FromBytes(ref memory, ref ip);
                    int addr = ReadInt32FromBytes(ref memory, ref ip);
                    int val = Pop();
                    if (val != condition)
                    {
                        ip = addr;
                        break;
                    }
                    break;
                }

            case Operations.BrGt:
                {
                    ip++;
                    int condition = ReadInt32FromBytes(ref memory, ref ip);
                    int addr = ReadInt32FromBytes(ref memory, ref ip);
                    int val = Pop();
                    if (val > condition)
                    {
                        ip = addr;
                        break;
                    }
                    break;
                }

            case Operations.BrGteq:
                {
                    ip++;
                    int condition = ReadInt32FromBytes(ref memory, ref ip);
                    int addr = ReadInt32FromBytes(ref memory, ref ip);
                    int val = Pop();
                    if (val >= condition)
                    {
                        ip = addr;
                        break;
                    }
                    break;
                }

            case Operations.BrLt:
                {
                    ip++;
                    int condition = ReadInt32FromBytes(ref memory, ref ip);
                    int addr = ReadInt32FromBytes(ref memory, ref ip);
                    int val = Pop();
                    if (val < condition)
                    {
                        ip = addr;
                        break;
                    }
                    break;
                }

            case Operations.BrLteq:
                {
                    ip++;
                    int condition = ReadInt32FromBytes(ref memory, ref ip);
                    int addr = ReadInt32FromBytes(ref memory, ref ip);
                    int val = Pop();
                    if (val <= condition)
                    {
                        ip = addr;
                        break;
                    }
                    break;
                }

            case Operations.BrRand:
                {
                    ip++;
                    int chance = memory[ip++];
                    int addr = ReadInt32FromBytes(ref memory, ref ip);
                    if (r.Next(0, 101) <= chance)
                    {
                        ip = addr;
                        break;
                    }
                    break;
                }

            default:
                throw new Exception("Not implemented!");
        }
    }

    public void Start()
    {
        while (running)
        {
            Execute();
        }
    }

    public void Push(int value)
    {
        //Console.WriteLine($"push {value}");
        unchecked
        {
            WriteInt32ToStack(ref memory, ref sp, value);
            sfsize += 4;
        }
    }

    public int Pop()
    {
        int result = ReadInt32FromStack(ref memory, ref sp);
        sfsize -= 4;
        //Console.WriteLine($"{result}");
        return result;
    }

    /// <summary>
    /// Returns an int32 from a given position in the instruction memory,
    /// and advances the instruction pointer as necessary
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    int ReadInt32FromBytes(ref byte[] arr, ref int ip)
    {
        int result = (arr[ip++] |
            (arr[ip++] << 8) |
            (arr[ip++] << 8 * 2) |
            (arr[ip++] << 8 * 3));
        //Console.WriteLine("read " + result);
        return result;
    }

    int ReadInt32FromStack(ref byte[] arr, ref int ip)
    {
        ip++;
        int result = (arr[ip++]) << 24 |
            (arr[ip++]) << 16 |
            (arr[ip++]) << 08 |
            (arr[ip]) << 00;
        //Console.WriteLine("read " + result);
        return result;
    }

    void WriteInt32ToStack(ref byte[] arr, ref int ip, int value)
    {
        arr[ip--] = (byte)((value >> 00) & 0xFF);
        arr[ip--] = (byte)((value >> 08) & 0xFF);
        arr[ip--] = (byte)((value >> 16) & 0xFF);
        arr[ip--] = (byte)((value >> 24) & 0xFF);
    }

    /// <summary>
    /// Returns an int16 from a given position in the instruction memory,
    /// and advances the instruction pointer as necessary
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    int ReadInt16FromBytes(ref byte[] arr, ref int ip)
    {
        int result = (arr[ip++] << 0 |
            (arr[ip++] << 8));
        //Console.WriteLine("read " + result);
        return result;
    }

    void WriteInt16FromBytes(ref byte[] arr, ref int ip, int value)
    {
        unchecked
        {
            arr[ip++] = (byte)((value >> 0) & 0xFF);
            arr[ip++] = (byte)((value >> 8) & 0xFF);
        }
    }

    void WriteInt32FromBytes(ref byte[] arr, ref int ip, int value)
    {
        unchecked
        {
            arr[ip++] = (byte)((value >> 0) & 0xFF);
            arr[ip++] = (byte)((value >> 8) & 0xFF);
            arr[ip++] = (byte)((value >> 16) & 0xFF);
            arr[ip++] = (byte)((value >> 24) & 0xFF);
        }
    }
}

