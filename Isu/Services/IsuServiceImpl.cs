using System.Collections.Generic;
using System.Linq;
using Isu.Entities;
using Isu.Tools;

namespace Isu.Services
{
    public class IsuServiceImpl : IIsuService
    {
        public const int MaxStudentsInGroupCount = 20;
        private Dictionary<GroupName, Group> _groups = new Dictionary<GroupName, Group>();
        private int _lastStudentId = 0;

        public Group AddGroup(GroupName name)
        {
            if (name == null)
                throw new IsuException("Student cannot be null");
            if (_groups.ContainsKey(name))
            {
                throw new IsuException("Group with such name already exists");
            }

            var group = new Group(name);
            _groups[name] = group;
            return group;
        }

        public Student AddStudent(Group group, string name)
        {
            if (group == null)
                throw new IsuException("Group cannot be null");
            if (name == null)
                throw new IsuException("Name cannot be null");
            if (group.Students.Count >= MaxStudentsInGroupCount)
                throw new IsuException($"Group {group.GroupName.StringName} is full");
            var student = new Student(++_lastStudentId, group, name);
            group.AddStudent(student);
            return student;
        }

        public Student GetStudent(int id)
        {
            foreach (Group group in _groups.Values)
            {
                foreach (Student student in group.Students)
                {
                    if (student.Id == id)
                        return student;
                }
            }

            throw new IsuException($"Student with id {id} was not found");
        }

        public Student FindStudent(string name)
        {
            var candidates = new List<Student>();
            foreach (Group group in _groups.Values)
            {
                foreach (Student student in group.Students)
                {
                    if (student.Name.Equals(name))
                        candidates.Add(student);
                }
            }

            if (candidates.Count == 0)
                return null;
            if (candidates.Count > 1)
                throw new IsuException("Multiple students found with such name: " + name);
            return candidates.First();
        }

        public List<Student> FindStudents(GroupName groupName)
        {
            var students = new List<Student>();
            if (_groups.ContainsKey(groupName))
            {
                HashSet<Student> studentsInGroup = _groups[groupName].Students;
                foreach (Student student in studentsInGroup)
                {
                    students.Add(student);
                }
            }

            return students;
        }

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            var students = new List<Student>();
            foreach (Group group in _groups.Values)
            {
                if (group.GetCourseNumber().Equals(courseNumber))
                {
                    students.AddRange(group.Students);
                }
            }

            return students;
        }

        public Group FindGroup(GroupName groupName)
        {
            return _groups[groupName];
        }

        public List<Group> FindGroups(CourseNumber courseNumber)
        {
            var groups = new List<Group>();
            foreach (Group group in _groups.Values)
            {
                if (group.GetCourseNumber().Equals(courseNumber))
                {
                    groups.Add(group);
                }
            }

            return groups;
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            if (student == null)
                throw new IsuException("Student cannot be null");
            if (newGroup == null)
                throw new IsuException("New group cannot be null");
            if (newGroup.Students.Count >= MaxStudentsInGroupCount)
                throw new IsuException($"Group {newGroup.GroupName.StringName} is full");
            student.Group.KickStudent(student);
            newGroup.AddStudent(student);
            student.Group = newGroup;
        }
    }
}