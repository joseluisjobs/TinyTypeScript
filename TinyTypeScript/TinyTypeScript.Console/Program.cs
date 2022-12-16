using TinyTypeScript.Lexer;
using TinyTypeScript.Parser;

Console.WriteLine("Hello, World!");
var code = File.ReadAllText("TestCode.txt").Replace(Environment.NewLine, "\n");
var input = new Input(code);
var scanner = new Scanner(input);

var parser = new Parser(scanner);

parser.Parse();
