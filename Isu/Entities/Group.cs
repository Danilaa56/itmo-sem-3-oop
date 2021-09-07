using System.Collections.Generic;
using System.Collections.Immutable;

namespace Isu.Entities
{
    public class Group
    {
        private HashSet<Student> _students = new HashSet<Student>();
        public Group(GroupName groupName)
        {
            GroupName = groupName;
        }

        public GroupName GroupName { get; }

        public void AddStudent(Student student)
        {
            _students.Add(student);
        }

        public void KickStudent(Student student)
        {
            _students.Remove(student);
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

        public ImmutableHashSet<Student> GetStudents()
        {
            return _students.ToImmutableHashSet();
        }
    }
}