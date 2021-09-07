using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
                throw new ArgumentNullException(nameof(name));
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
                throw new ArgumentNullException(nameof(group));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (group.GetStudents().Count >= MaxStudentsInGroupCount)
                throw new IsuException($"Group {group.GroupName.StringName} is full");
            var student = new Student(++_lastStudentId, group, name);
            group.AddStudent(student);
            return student;
        }

        public Student GetStudent(int id)
        {
            foreach (Group group in _groups.Values)
            {
                return group.GetStudents().FirstOrDefault(s => s.Id == id);
            }

            throw new IsuException($"Student with id {id} was not found");
        }

        public Student FindStudent(string name)
        {
            return _groups.Values.Select(group => group.GetStudents().FirstOrDefault(s => s.Name.Equals(name))).FirstOrDefault();
        }

        public List<Student> FindStudents(GroupName groupName)
        {
            var students = new List<Student>();
            if (_groups.TryGetValue(groupName, out Group group))
            {
                students.AddRange(group.GetStudents());
            }

            return students;
        }

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            var students = new List<Student>();
            foreach (Group group in _groups.Values.Where(group => group.GetCourseNumber() == courseNumber))
            {
                students.AddRange(group.GetStudents());
            }

            return students;
        }

        public Group FindGroup(GroupName groupName)
        {
            return _groups[groupName];
        }

        public List<Group> FindGroups(CourseNumber courseNumber)
        {
            return new List<Group>(_groups.Values.Where(group => group.GetCourseNumber() == courseNumber));
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));
            if (newGroup == null)
                throw new ArgumentNullException(nameof(newGroup));
            if (newGroup.GetStudents().Count >= MaxStudentsInGroupCount)
                throw new IsuException($"Group {newGroup.GroupName.StringName} is full");
            student.Group.KickStudent(student);
            newGroup.AddStudent(student);
            student.Group = newGroup;
        }
    }
}