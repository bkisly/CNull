using var cnull = new CNull.Core.CNull(s =>
{
    Console.Write(s);
    return Console.ReadLine();
}, Console.WriteLine, Console.WriteLine);

Console.WriteLine("Hello, World!");

await cnull.ExecuteFromFileAsync(@"C:\Users\bkisl\Desktop\test.cnull");
