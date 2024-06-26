using CNull.ErrorHandler.Errors;
using CNull.Interpreter.Tests.Data;
using CNull.Interpreter.Tests.Helpers;
using Moq;

namespace CNull.Interpreter.Tests
{
    public class TypesResolverTests(TypesResolverHelpersFixture fixture) : IClassFixture<TypesResolverHelpersFixture>
    {
        [Theory, ClassData(typeof(AssignmentData))]
        public void CanResolveAssignments(object? left, object? right, object? expectedResult)
        {
            var result = fixture.TypesResolver.ResolveAssignment(left, right, 0);
            Assert.Equal(expectedResult, result);
            fixture.ErrorHandler.Verify(e => e.RaiseSemanticError(It.IsAny<ISemanticError>()), Times.Never);
        }

        [Fact]
        public void CanResolveAddition()
        {

        }

        [Fact]
        public void CanResolveSubtraction()
        {

        }

        [Fact]
        public void CanResolveMultiplication()
        {

        }

        [Fact]
        public void CanResolveDivision()
        {

        }

        [Fact]
        public void CanResolveModulo()
        {

        }

        [Fact]
        public void CanResolveRelational()
        {

        }

        [Fact]
        public void CanResolveEquality()
        {

        }
    }
}