using CNull.Common.Configuration;
using CNull.ErrorHandler;
using CNull.Source;

namespace CNull.Lexer.ServicesContainers
{
    public class LexerStateServicesContainer(ICodeSource source, IErrorHandler errorHandler, ICNullConfiguration configuration) 
        : ILexerStateServicesContainer
    {
        public ICodeSource CodeSource => source;
        public IErrorHandler ErrorHandler => errorHandler;
        public ICNullConfiguration CNullConfiguration => configuration;
    }
}
