using System.Text.RegularExpressions;
using Isu.Tools;

namespace Isu.Entities
{
    public class GroupName
    {
        public GroupName(string stringName)
        {
            if (!Regex.IsMatch(stringName, "^[A-Z][1-4][1-4][0-9]{2,3}[c]*$"))
            {
                throw new IsuException($"Illegal group name: {stringName}");
            }

            StringName = stringName;
        }

        public string StringName { get; }

        public override int GetHashCode()
        {
            return StringName.GetHashCode();
        }
    }
}