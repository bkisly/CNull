using CNull.Common;

namespace CNull.Source.Tests.Data
{
    internal class PositionCounterData : TheoryData<string, int, Position>
    {
        public PositionCounterData()
        {
            Add("sample\ninput", 9, new Position(2, 2));
            Add("sample\r\ninput", 10, new Position(2, 2));
            Add("sample\n\rinput", 10, new Position(2, 2));
            Add("sample\n\rinp\n\rut", 100, new Position(3, 2));
            Add("", 100, default);
            Add("\n\n\n\n\n\n\n", 100, new Position(7, 1));
            Add("\r\n\r\n\r\n\r\n\r\n\r\n\r\n", 100, new Position(7, 1));
            Add("\n\r\n\r\n\r\n\r\n\r\n\r\n\r", 100, new Position(8, 0));
        }
    }
}
