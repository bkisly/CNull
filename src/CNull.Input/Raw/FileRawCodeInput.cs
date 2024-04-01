using CNull.Common.Events.Args;
using CNull.Common.Mediators;
using CNull.Source.Repositories;

namespace CNull.Source.Raw
{
    /// <summary>
    /// Handles raw code input from files.
    /// </summary>
    public class FileRawCodeInput : RawCodeInput
    {
        private readonly ICoreComponentsMediator _mediator;

        public FileRawCodeInput(IInputRepository inputRepository, ICoreComponentsMediator mediator) : base(inputRepository)
        {
            _mediator = mediator;
            _mediator.FileInputRequested += OnFileInputRequested;
        }

        private void OnFileInputRequested(object? sender, FileInputRequestedEventArgs e)
        {
            try
            {
                InputRepository.Setup(new FileStream(e.SourcePath, FileMode.Open));
            }
            catch (IOException)
            {
                throw new NotImplementedException("Implement error when file input could not be read.");
            }
        }
    }
}
