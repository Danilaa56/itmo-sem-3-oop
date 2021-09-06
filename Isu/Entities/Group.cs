using System.Collections.Generic;

namespace Isu.Entities
{
    public class Group
    {
        public Group(GroupName groupName)
        {
            GroupName = groupName;
        }

        public GroupName GroupName { get; }
        public HashSet<Student> Students { get; } = new HashSet<Student>();

        public void AddStudent(Student student)
        {
            Students.Add(student);
        }

        public void KickStudent(Student student)
        {
            Students.Remove(student);
        }

        public override int GetHashCode()
        {
            return GroupName.GetHashCode();
        }

        public CourseNumber GetCourseNumber()
        {
            char c = GroupName.StringName.ToCharArray()[2];
            return (CourseNumber)(c - '0');
        }
    }
}