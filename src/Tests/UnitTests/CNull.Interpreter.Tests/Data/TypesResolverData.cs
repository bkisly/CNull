using System.Globalization;

namespace CNull.Interpreter.Tests.Data
{
    public class AssignmentData : TheoryData<object?, object?, object?>
    {
        public AssignmentData()
        {
            AddNullAssignmentData();
            AddMatchingTypeAssignmentData();
            AddImplicitlyConvertableAssignmentData();
        }

        public void AddNullAssignmentData()
        {
            Add(null, null, null);
            Add("abcde", null, null);
            Add(1, null, null);
            Add(1f, null, null);
            Add(true, null, null);
            Add('a', null, null);
            Add(new Dictionary<int, string>(), null, null);
        }

        public void AddMatchingTypeAssignmentData()
        {
            Add("a", "b", "b");
            Add(1, 2, 2);
            Add('a', 'b', 'b');
            Add(true, false, false);
        }

        public void AddImplicitlyConvertableAssignmentData()
        {
            Add(1, 2.3f, 2);
            Add(2.3f, 1, 1);
            Add("a", 1, "1");
            Add("a", 1.3f, 1.3f.ToString(CultureInfo.CurrentCulture));
            Add("a", 'a', "a");
        }
    }
}
