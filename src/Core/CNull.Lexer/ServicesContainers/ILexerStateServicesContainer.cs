using CNull.Common.Configuration;
using CNull.ErrorHandler;
using CNull.Source;

namespace CNull.Lexer.ServicesContainers
{
    /// <summary>
    /// Aggregates services that are used in lexer state related logic.
    /// </summary>
    public interface ILexerStateServicesContainer
    {
        /// <summary>
        /// <inheritdoc cref="ICodeSource"/>
        /// </summary>
        public ICodeSource CodeSource { get; }

        /// <summary>
        /// <inheritdoc cref="IErrorHandler"/>
        /// </summary>
        public IErrorHandler ErrorHandler { get; }

        /// <summary>
        /// <inheritdoc cref="ICompilerConfiguration"/>
        /// </summary>
        public ICompilerConfiguration CompilerConfiguration { get; }
    }
}
