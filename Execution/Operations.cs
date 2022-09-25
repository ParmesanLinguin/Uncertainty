namespace Uncertainty.Execution;

public enum Operations : byte
{
    /// <summary>
    /// <b>Args:</b> None <br />
    /// <b>Stack:</b> None -> None <br />
    /// <b>No-op</b> <br />
    /// No operation
    /// </summary>
    Noop,

    /// <summary>
    /// <b>Args:</b> None <br />
    /// <b>Stack:</b> None -> Never <br />
    /// <b>Halt</b> <br />
    /// Terminate the program
    /// </summary>
    Halt,

    /// <summary>
    /// <b>Args:</b> Unknown <br />
    /// <b>Stack:</b> Unknown <br />
    /// <b>Debug</b> <br />
    /// Run the user-specified debug callback
    /// </summary>
    Debug,

    /// <summary>
    /// <b>Args:</b> int32 <br />
    /// <b>Stack:</b> None -> int32 <br />
    /// <b>Push</b> <br />
    /// Push an int32 to the stack
    /// </summary>
    Push,

    /// <summary>
    /// <b>Args:</b> None <br />
    /// <b>Stack:</b> int32 -> None <br />
    /// <b>Pop</b> <br />
    /// Pop an int32 from the stack
    /// </summary>
    Pop,

    /// <summary>
    /// <b>Args:</b> None <br />
    /// <b>Stack:</b> int32 a -> int32 a, int32 a <br />
    /// <b>Pop</b> <br />
    /// Duplicate an int32 on the stack
    /// </summary>
    Dup,

    /// <summary>
    /// <b>Args:</b> None <br />
    /// <b>Stack:</b> int32 a, int32 b -> int32 b, int32 a <br />
    /// <b>Pop</b> <br />
    /// Swap two int32s on the stack
    /// </summary>
    Swap,

    /// <summary>
    /// <b>Args:</b> None <br />
    /// <b>Stack:</b> int32 a, int32 b, int32 c, -> int32 b, int32 c, int32 a <br />
    /// <b>Pop</b> <br />
    /// Rotate 3 stack elements left
    /// </summary>
    LRotate,

    /// <summary>
    /// <b>Args:</b> None <br />
    /// <b>Stack:</b> int32 a, int32 b, int32 c, -> int32 c, int32 a, int32 b <br />
    /// <b>Pop</b> <br />
    /// Rotate 3 stack elements left
    /// </summary>
    RRotate,

    /// <summary>
    /// <b>Args:</b> int32 <br />
    /// <b>Stack:</b> int32 -> None <br />
    /// <b>Store Int32</b> <br />
    /// Pop an int32 from the stack and store it at the provided address
    /// </summary>
    Stor4,

    /// <summary>
    /// <b>Args:</b> None <br />
    /// <b>Stack:</b int32 value, int32 addr -> None <br />
    /// <b>Stack Store Int32</b> <br />
    /// Pop an int32 from the stack and store it at the stack-provided address
    /// </summary>
    SStor4,

    /// <summary>
    /// <b>Args:</b> int32 <br />
    /// <b>Stack:</b> None -> None <br />
    /// <b>Load Int32</b> <br />
    /// Load an int32 from the provided address and push it to the stack
    /// </summary>
    Load4,

    /// <summary>
    /// <b>Args:</b> None <br />
    /// <b>Stack:</b> int32 -> None <br />
    /// <b>Stack Load Int32</b> <br />
    /// Load an int32 from the stack-provided address and push it to the stack
    /// </summary>
    SLoad4,

    /// <summary>
    /// <b>Args:</b> int32 bytes<br />
    /// <b>Stack:</b> None -> None<br />
    /// <b>Store Local Int32</b> <br />
    /// Allocate the specified number of bytes for local variable storage.
    /// </summary>
    Allocloc,

    /// <summary>
    /// <b>Args:</b> int16 index <br />
    /// <b>Stack:</b> int32 -> None<br />
    /// <b>Store Local Int32</b> <br />
    /// Pop an int32 from the stack and store it as a local variable at the provided index.
    /// </summary>
    StorLoc4,

    /// <summary>
    /// <b>Args:</b> int16 index <br />
    /// <b>Stack:</b> None -> int32<br />
    /// <b>Load Local Int32</b> <br />
    /// Load an int32 from the index and push it to the stack.
    /// </summary>
    LoadLoc4,

    /// <summary>
    /// <b>Args:</b> int16 index <br />
    /// <b>Stack:</b> None -> int32<br />
    /// <b>Load Argument Int32</b> <br />
    /// Load an argument from the index and push it to the stack.
    /// </summary>
    LoadArg4,

    /// <summary>
    /// <b>Args:</b> None <br />
    /// <b>Stack:</b> int32, int32 -> int32 <br />
    /// <b>Add</b> <br />
    /// Add two int32s from the stack and push the result
    /// </summary>
    Add,

    /// <summary>
    /// <b>Args:</b> None <br />
    /// <b>Stack:</b> int32 a, int32 b -> int32 <br />
    /// <b>Sub</b> <br />
    /// Sub two int32s (a - b) from the stack and push the result
    /// </summary>
    Sub,

    /// <summary>
    /// <b>Args:</b> int32 addr <br />
    /// <b>Stack:</b> int32 bool <br />
    /// <b>Jump</b> <br />
    /// Jump to the specified address
    /// </summary>
    Jmp,

    /// <summary>
    /// <b>Args:</b> int32 condition, int32 addr <br />
    /// <b>Stack:</b> int32 bool <br />
    /// <b>Branch Equals</b> <br />
    /// Jump to the specified address if the value is equal to the condition
    /// </summary>
    BrEq,

    /// <summary>
    /// <b>Args:</b> int32 condition, int32 addr <br />
    /// <b>Stack:</b> int32 bool <br />
    /// <b>Branch Not Equals</b> <br />
    /// Jump to the specified address if the value is not equal to the condition
    /// </summary>
    BrNeq,

    /// <summary>
    /// <b>Args:</b> int32 condition, int32 addr <br />
    /// <b>Stack:</b> int32 bool <br />
    /// <b>Branch Greater Than</b> <br />
    /// Jump to the specified address if the value is greater than the condition
    /// </summary>
    BrGt,

    /// <summary>
    /// <b>Args:</b> int32 condition, int32 addr <br />
    /// <b>Stack:</b> int32 bool <br />
    /// <b>Branch Greater Than Equals</b> <br />
    /// Jump to the specified address if the value is greater than or equal to the condition
    /// </summary>
    BrGteq,

    /// <summary>
    /// <b>Args:</b> int32 condition, int32 addr <br />
    /// <b>Stack:</b> int32 bool <br />
    /// <b>Branch Less Than</b> <br />
    /// Jump to the specified address if the value is less than the condition
    /// </summary>
    BrLt,

    /// <summary>
    /// <b>Args:</b> int32 condition, int32 addr <br />
    /// <b>Stack:</b> int32 bool <br />
    /// <b>Branch Less Than Equals</b> <br />
    /// Jump to the specified address if the value is less than or equal to the condition
    /// </summary>
    BrLteq,

    /// <summary>
    /// <b>Args:</b> int8 chance, int32 addr <br />
    /// <b>Stack:</b> None -> None <br />
    /// <b>Branch Random</b> <br />
    /// Jump to the specified address if the value is not equal. Chance between 0% to 100%, inclusive
    /// </summary>
    BrRand,

    /// <summary>
    /// <b>Args:</b> int32 addr <br />
    /// <b>Stack:</b> ? -> stackframe <br />
    /// <b>Call</b> <br />
    /// Jump to the specified address and push a new stack frame
    /// </summary>
    Call,

    /// <summary>
    /// <b>Args:</b> None<br />
    /// <b>Stack:</b> stackframe -> ? <br />
    /// <b>Return</b> <br />
    /// Return to the address from the last stack frame and pop the frame.
    /// </summary>
    Ret,

    /// <summary>
    /// <b>Args:</b> None<br />
    /// <b>Stack:</b> stackframe, int32 a -> int32 a<br />
    /// <b>Return</b> <br />
    /// Return to the address from the last stack frame and pop the frame, and add the return element
    /// </summary>
    Ret4,
}