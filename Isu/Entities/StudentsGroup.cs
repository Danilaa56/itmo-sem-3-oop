using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Isu.Tools;

namespace Isu.Entities
{
    public abstract class StudentsGroup
    {
        private readonly HashSet<Student> _students = new HashSet<Student>();
        private readonly List<Classes> _classes = new List<Classes>();

        protected StudentsGroup(int sizeLimit)
        {
            if (sizeLimit < 0)
                throw new ArgumentException("Size limit cannot be negative", nameof(sizeLimit));
            SizeLimit = sizeLimit;
        }

        public int SizeLimit { get; }

        public virtual void AddStudent(Student student)
        {
            if (student is null)
                throw new ArgumentNullException(nameof(student));
            if (_students.Contains(student))
                throw new IsuException("Student is already in this group");
            if (_students.Count >= SizeLimit)
                throw new IsuException("The group is full");
            _students.Add(student);
        }

        public virtual void KickStudent(Student student)
        {
            if (!_students.Remove(student ?? throw new ArgumentNullException(nameof(student))))
                throw new IsuException("There is no such student in this group");
        }

        public ImmutableList<Student> GetStudents()
        {
            return _students.ToImmutableList();
        }

        public virtual void AddClasses(Classes classes)
        {
            if ((classes ?? throw new ArgumentNullException(nameof(classes))).Group != this)
                throw new IsuException("Classes group reference differs from this group");
            if (_classes.Any(existingClasses => !existingClasses.NotIntersecting(classes)))
                throw new IsuException("This group has classes intersecting with the adding one");
            _classes.Add(classes);
        }

        public ImmutableList<Classes> GetClasses()
        {
            return _classes.ToImmutableList();
        }

        public abstract string GetName();
    }
}