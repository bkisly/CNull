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
    }
}
