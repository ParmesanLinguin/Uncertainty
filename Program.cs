using Uncertainty.Tokenization;
using Uncertainty.Parsing;
using Uncertainty.Parsing.Debug;
using Uncertainty.Parsing.AST.Nodes;
using Uncertainty.Parsing.HIR;

if (args.Length == 0)
{
    Console.WriteLine("Please provide an input file");
    return 1;
}

string inputFile = string.Join("", args);
if (!File.Exists(inputFile))
{
    Console.WriteLine("File not found!");
    return 1;
}

//if (new Random().Next(0, 2) == 1)
//{
//    Console.WriteLine("Thank you for trying to compile today.");
//    return 0;
//}

using var file = File.Open(inputFile, FileMode.Open, FileAccess.Read);

BufferedUTF8Converter converter = new(file);
//foreach (var c in converter)
//{
//    Console.WriteLine($"{(int)c} `{c}`");
//}

Tokenizer lexer = new Tokenizer(converter);
var pretokens = lexer.Tokenize();
var tokens = IndentationProcessor.ProcessIndentation(pretokens).ToList();

foreach (var token in tokens)
{
    Console.WriteLine(token);
}

Parser p = new Parser(tokens);
var node = p.Parse();
node.PrintPretty("", true);
var hirConverter = new AstToHir(node);
var n = hirConverter.ConvertProgram();
Console.WriteLine(n.Modules[0].Functions[0]);
return 0;

//using System.Diagnostics;
//using System.Text;
//using Uncertainty.Execution;
//using Uncertainty.Execution.Assembler;

//var assemblerTok = new AssemblerTokenizer(@"
//.const iterations 10 4;start:;push 0;loop:;call :fib;debug;push 1;add;dup;brgt %iterations,:end;jmp :loop;end:;halt;fib:;allocloc 8;loadarg4 0;dup;brgteq 2,:fibbody;ret4;fibbody:;dup;storloc4 0;storloc4 1;loadloc4 0;push 1;sub;storloc4 0;loadloc4 1;push 2;sub;storloc4 1;loadloc4 0;call :fib;storloc4 0;pop;loadloc4 1;call :fib;storloc4 1;pop;loadloc4 1;loadloc4 0;add;ret4;
//");

//var sw = new Stopwatch();
//sw.Start();

//var tokens = assemblerTok.Tokenize();
//tokens.ForEach(t => Console.WriteLine(t));

//var parser = new AssemblerParser(tokens);
//var result = parser.Parse();

//sw.Stop();
//Console.WriteLine($"Compilation took {sw.ElapsedMilliseconds} ms");
//Console.WriteLine($"Program bytes: {string.Join(" ", result.Select(x => $"{x:X2}"))}");

//byte[] memory = new byte[10000];
//result.CopyTo(memory, 0);

//var interp = new Runtime(memory, (rt) =>
//{
//    Console.WriteLine(rt.Pop4());
//});
//sw.Restart();
//interp.Start();

//sw.Stop();
//Console.WriteLine($"Execution took {sw.Elapsed.TotalMilliseconds} ms");