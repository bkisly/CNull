using CNull.ErrorHandler;
using CNull.Interpreter.Context;
using CNull.Interpreter.Resolvers;
using Moq;

namespace CNull.Semantics.Symbols.Tests.Helpers
{
    public class TypesResolverHelpersFixture
    {
        public Mock<IErrorHandler> ErrorHandler { get; }
        public Mock<IExecutionEnvironment> Environment { get; }
        public TypesResolver TypesResolver { get; }

        public TypesResolverHelpersFixture()
        {
            ErrorHandler = new Mock<IErrorHandler>();
            Environment = new Mock<IExecutionEnvironment>();
            Environment.SetupGet(e => e.CurrentModule).Returns("Program");

            TypesResolver = new TypesResolver(Environment.Object, ErrorHandler.Object);
        }
    }
}
