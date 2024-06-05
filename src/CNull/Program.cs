using System.CommandLine;
using CNull.Interpreter;

var rootCommand = new RootCommand("Command-line interface for C? language interpreter.");
var pathOption = new Argument<string>(name: "path", description: "Path to the file or root directory to execute.", getDefaultValue: () => Environment.CurrentDirectory);
var argsOption = new Option<IEnumerable<string>>(name: "--args", description: "Positional arguments to be passed to the program.", getDefaultValue: () => [])
{
    AllowMultipleArgumentsPerToken = true
};

rootCommand.AddArgument(pathOption);
rootCommand.AddOption(argsOption);

rootCommand.SetHandler(Execute, pathOption, argsOption);

return await rootCommand.InvokeAsync(args);

static async Task Execute(string path, IEnumerable<string> args)
{
    using var cnull = new CNullCore(args.ToArray(), Console.ReadLine, Console.Write, Console.Write);
    await cnull.ExecuteFromFileAsync(path);
}