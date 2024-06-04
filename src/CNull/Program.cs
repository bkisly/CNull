using CNull.Interpreter;

using var cnull = new CNullCore(Console.ReadLine, Console.Write, Console.Write);
await cnull.ExecuteFromFileAsync(args.FirstOrDefault() ?? string.Empty);
