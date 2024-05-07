using CNull.Interpreter;

using var cnull = new CNullCore(_ => Console.ReadLine(), Console.WriteLine, Console.WriteLine);
await cnull.ExecuteFromFileAsync(args.FirstOrDefault() ?? string.Empty);
