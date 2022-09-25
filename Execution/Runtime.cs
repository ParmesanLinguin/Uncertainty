using System.Collections.ObjectModel;

namespace Uncertainty.Execution;

public class Runtime
{
    private byte[] memory;

    int stackPtr;
    public int StackPointer { get => stackPtr; private set => stackPtr = value; }

    int framePtr;
    public int FramePtr { get => framePtr; private set => framePtr = value; }

    int instrPtr;
    public int InstrPtr { get => InstrPtr; private set => instrPtr = value; }

    int frameSize;

    public ReadOnlyCollection<byte> Memory { get; private set; }

    Action<Runtime>? debug;
    Random random;

    bool halted = false;

    public Runtime(byte[] memory, Action<Runtime>? debugCallback = null)
    {
        this.memory = memory;
        Memory = new(memory);
        instrPtr = 0;
        stackPtr = memory.Length - 1;
        framePtr = stackPtr;
        frameSize = 0;
        random = new();
        debug = debugCallback;
    }
    
    /// <summary>
    /// Read a single instruction, and execute it
    /// </summary>
    public void Execute()
    {
        var fn = ((byte x, int i) =>
        {
            //return $"{(x.Length * 4 - i):X2}: {string.Join(" ", x.Select(x => $"{x:X2}"))}";
            if (i + 1 == memory.Length - stackPtr)
            {
                return $"\x1B[32m{x:X2}\x1B[0m";
            }
            else if (i + 1 == memory.Length - framePtr)
            {
                return $"\x1B[31m{x:X2}\x1B[0m";
            }
            else
            {
                return $"{x:X2}";
            }
        });
        Operations op = (Operations) memory[instrPtr++];
        //Console.WriteLine($"{string.Join(" ", memory.Reverse().Take(8 * 8).Select((x, i) => fn(x, i)))}");
        //Console.WriteLine(op);

        switch (op)        
        {
            case Operations.Noop:
                break;

            case Operations.Halt:
                halted = true;
                break;

            case Operations.Debug:
                {
                    if (debug is null) throw new Exception("No debug handler specified");
                    debug.Invoke(this);
                    break;
                }
                

            case Operations.Push:
                {
                    int value = Read4(ref instrPtr);
                    Push4(value);
                    break;
                }
                

            case Operations.Pop:
                {
                    _ = Pop4();
                    break;
                }


            case Operations.Dup:
                {
                    int value = Pop4();
                    Push4(value);
                    Push4(value);
                    break;
                }

            case Operations.Swap:
                {
                    int b = Pop4();
                    int a = Pop4();
                    Push4(b);
                    Push4(a);
                    break;
                }

            case Operations.LRotate:
                {
                    int c = Pop4();
                    int b = Pop4();
                    int a = Pop4();
                    Push4(b);
                    Push4(c);
                    Push4(a);
                    break;
                }

            case Operations.RRotate:
                {
                    int c = Pop4();
                    int b = Pop4();
                    int a = Pop4();
                    Push4(c);
                    Push4(a);
                    Push4(b);
                    break;
                }

            case Operations.Allocloc:
                {
                    int count = Read4(ref instrPtr);
                    stackPtr -= count;
                    frameSize += count;
                    break;
                }

            case Operations.StorLoc4:
                {
                    int index = Read2(ref instrPtr);
                    int value = Pop4();
                    int location = framePtr - (4 * (index + 1)) + 1;
                    Write4(ref location, value);
                    break;
                }

            case Operations.LoadLoc4:
                {
                    int index = Read2(ref instrPtr);
                    int location = framePtr - (4 * (index + 1)) + 1;
                    int value = Read4(ref location);
                    Push4(value);
                    break;
                }

            case Operations.Add:
                {
                    int b = Pop4();
                    int a = Pop4();
                    Push4(a + b);
                    break;
                }

            case Operations.Sub:
                {
                    int b = Pop4();
                    int a = Pop4();
                    Push4(a - b);
                    break;
                }

            case Operations.Jmp:
                {
                    int address = Read4(ref instrPtr);
                    instrPtr = address;
                    break;
                }

            case Operations.BrEq:
                {
                    int condition = Read4(ref instrPtr);
                    int address = Read4(ref instrPtr);
                    int val = Pop4();
                    if (val == condition)
                    {
                        instrPtr = address;
                    }
                    break;
                }

            case Operations.BrNeq:
                {
                    int condition = Read4(ref instrPtr);
                    int address = Read4(ref instrPtr);
                    int val = Pop4();
                    if (val != condition)
                    {
                        instrPtr = address;
                    }
                    break;
                }

            case Operations.BrGt:
                {
                    int condition = Read4(ref instrPtr);
                    int address = Read4(ref instrPtr);
                    int val = Pop4();
                    if (val > condition)
                    {
                        instrPtr = address;
                    }
                    break;
                }

            case Operations.BrGteq:
                {
                    int condition = Read4(ref instrPtr);
                    int address = Read4(ref instrPtr);
                    int val = Pop4();
                    if (val >= condition)
                    {
                        instrPtr = address;
                    }
                    break;
                }

            case Operations.BrLt:
                {
                    int condition = Read4(ref instrPtr);
                    int address = Read4(ref instrPtr);
                    int val = Pop4();
                    if (val < condition)
                    {
                        instrPtr = address;
                    }
                    break;
                }

            case Operations.BrLteq:
                {
                    int condition = Read4(ref instrPtr);
                    int address = Read4(ref instrPtr);
                    int val = Pop4();
                    if (val <= condition)
                    {
                        instrPtr = address;
                    }
                    break;
                }

            case Operations.BrRand:
                {
                    int chance = Read1(ref instrPtr);
                    int address = Read1(ref instrPtr);
                    if (random.Next(0, 101) <= chance)
                    {
                        instrPtr = address;
                    }
                    break;
                }

            case Operations.LoadArg4:
                {
                    int index = Read2(ref instrPtr);
                    int loc = framePtr + 4 + 4 + 4 + (index * 4);
                    int val = StackRead4(ref loc);
                    Push4(val);
                    break;
                }

            case Operations.Call:
                {
                    int address = Read4(ref instrPtr);
                    PushCallstack();
                    instrPtr = address;
                    break;
                }
            case Operations.Ret:
                {
                    PopCallstack();
                    break;
                }
            case Operations.Ret4:
                {
                    int value = Pop4();
                    PopCallstack();
                    Push4(value);
                    break;
                }

            default:
                throw new Exception("Not implemented!");
        }
    }

    public void Start()
    {
        while (!halted)
        {
            Execute();
        }
    }

    private void PushCallstack()
    {
        Push4(framePtr);
        Push4(instrPtr);
        Push4(frameSize);
        framePtr = stackPtr;
        frameSize = 0;
    }

    private void PopCallstack()
    {
        stackPtr = framePtr;
        frameSize = Pop4();
        instrPtr = Pop4();
        framePtr = Pop4();   
    }

    private void Push4(int value)
    {
        memory[stackPtr--] = (byte)((value >> 8 * 0) & 0xFF);
        memory[stackPtr--] = (byte)((value >> 8 * 1) & 0xFF);
        memory[stackPtr--] = (byte)((value >> 8 * 2) & 0xFF);
        memory[stackPtr--] = (byte)((value >> 8 * 3) & 0xFF);
        frameSize += 4;
    }

    public int Pop4()
    {
        int result = memory[++stackPtr] << (8 * 3) |
                     memory[++stackPtr] << (8 * 2) |
                     memory[++stackPtr] << (8 * 1) |
                     memory[++stackPtr] << (8 * 0);
        frameSize -= 4;
        return result;
    }
    
    public int StackRead4(ref int location)
    {
        int result = memory[++location] << (8 * 3) |
             memory[++location] << (8 * 2) |
             memory[++location] << (8 * 1) |
             memory[++location] << (8 * 0);
        return result;
    }

    public void StackWrite4(ref int location, int value)
    {
        memory[location--] = (byte)((value >> 8 * 0) & 0xFF);
        memory[location--] = (byte)((value >> 8 * 1) & 0xFF);
        memory[location--] = (byte)((value >> 8 * 2) & 0xFF);
        memory[location--] = (byte)((value >> 8 * 3) & 0xFF);
    }

    private void Write4(ref int location, int value)
    {
        memory[location++] = (byte)((value >> 8 * 0) & 0xFF);
        memory[location++] = (byte)((value >> 8 * 1) & 0xFF);
        memory[location++] = (byte)((value >> 8 * 2) & 0xFF);
        memory[location++] = (byte)((value >> 8 * 3) & 0xFF);
    }

    public int Read4(ref int location)
    {
        int result = memory[location++] << (8 * 0) |
             memory[location++] << (8 * 1) |
             memory[location++] << (8 * 2) |
             memory[location++] << (8 * 3);
        return result;
    }

    public short Read2(ref int location)
    {
        short result = (short)(memory[location++] << (8 * 0) |
             memory[location++] << (8 * 1));
        return result;
    }

    public byte Read1(ref int location)
    {
        byte result = memory[location++];
        return result;
    }
}
