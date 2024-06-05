using CNull.IntegrationTests.AcceptanceTests.Fixtures;

namespace CNull.IntegrationTests.AcceptanceTests
{
    public class InterpreterAcceptanceTests
    {
        [Fact]
        public void CanPerformOperationsOnUserInput()
        {
            const string input = """
                                 import CNull.Console.Write;
                                 import CNull.Console.ReadLine;
                                 import CNull.Converters.StringToInt;

                                 // this is a sample comment

                                 void Main(dict<int, string> args)
                                 {
                                     int sum = Sum();
                                     Write(sum);
                                 }
                                 
                                 int Sum()
                                 {
                                    try
                                    {
                                        int a = StringToInt(ReadLine());
                                        int b = StringToInt(ReadLine());
                                        return a + b;
                                    }
                                    catch(e e == "FormatException")
                                    {
                                        return null;
                                    }
                                 }
                                 """;

            var output = new List<string>();
            var inputQueue = new Queue<string>(["14", "25"]);
            var expectedOutput = new[] { "39" };

            var interpreter = InterpreterDependenciesFixture.BuildInterpreter(input);
            interpreter.Execute([], inputQueue.Dequeue, output.Add);

            Assert.Equivalent(expectedOutput, output);
        }

        [Fact]
        public void CanCatchNullAccess()
        {
            const string input = """
                                 import CNull.Console.Write;
                                 import CNull.Console.ReadLine;
                                 import CNull.Converters.StringToInt;

                                 // this is a sample comment

                                 void Main(dict<int, string> args)
                                 {
                                     try
                                     {
                                        int sum = Sum();
                                        Write(sum);
                                     }
                                     catch(e e == "NullValueException")
                                     {
                                        Write(e + " - caught null");
                                     }
                                 }

                                 int Sum()
                                 {
                                    try
                                    {
                                        int a = StringToInt(ReadLine());
                                        int b = StringToInt(ReadLine());
                                        return a + null;
                                    }
                                    catch(e e == "FormatException")
                                    {
                                        return null;
                                    }
                                 }
                                 """;

            var output = new List<string>();
            var inputQueue = new Queue<string>(["14", "25"]);
            var expectedOutput = new[] { "NullValueException - caught null" };

            var interpreter = InterpreterDependenciesFixture.BuildInterpreter(input);
            interpreter.Execute([], inputQueue.Dequeue, output.Add);

            Assert.Equivalent(expectedOutput, output);
        }

        [Fact]
        public void CanProcessDictionaryByReference()
        {
            const string input = """
                                 import CNull.Console.Write;
                                 import CNull.Console.ReadLine;
                                 import CNull.Converters.StringToInt;

                                 // this is a sample comment

                                 void Main(dict<int, string> args)
                                 {
                                     ProcessDict(args);
                                     Write(args.Get(0)?);
                                     Write(args.Get(1));
                                     Write(args.Get(2));
                                 }

                                 void ProcessDict(dict<int, string> d)
                                 {
                                    d.Add(2, "third");
                                    d.Set(1, "modified");
                                    d.Remove(0);
                                 }
                                 """;

            var output = new List<string>();
            var inputQueue = new Queue<string>(["14", "25"]);
            var expectedOutput = new[] { "True", "modified", "third" };

            var interpreter = InterpreterDependenciesFixture.BuildInterpreter(input);
            interpreter.Execute([], inputQueue.Dequeue, output.Add);

            Assert.Equivalent(expectedOutput, output);
        }
    }
}
