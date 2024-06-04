using CNull.Interpreter;

using var cnull = new CNullCore(Console.ReadLine, Console.WriteLine, Console.WriteLine);
await cnull.ExecuteFromFileAsync(args.FirstOrDefault() ?? string.Empty);
