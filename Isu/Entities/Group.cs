namespace Isu.Entities
{
    public class Group : StudentsGroup
    {
        public Group(GroupName groupName, int sizeLimit)
            : base(sizeLimit)
        {
            GroupName = groupName;
        }

        public GroupName GroupName { get; }

        public override int GetHashCode()
        {
            return GroupName.GetHashCode();
        }

        public CourseNumber GetCourseNumber()
        {
            char c = GroupName.StringName.ToCharArray()[2];
            return (CourseNumber)(c - '0');
        }

        public override string GetName()
        {
            return GroupName.StringName;
        }
    }
}