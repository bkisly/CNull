using CNull.Interpreter;

using var cnull = new CNullCore(args.Skip(1).ToArray(), Console.ReadLine, Console.Write, Console.Write);
await cnull.ExecuteFromFileAsync(args.FirstOrDefault() ?? string.Empty);
