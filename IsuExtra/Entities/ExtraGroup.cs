using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Isu.Entities;
using Isu.Tools;

namespace IsuExtra.Entities
{
    public class ExtraGroup : StudentsGroup
    {
        private readonly HashSet<Student> _students = new HashSet<Student>();

        public ExtraGroup(ExtraCourse course, string name, int sizeLimit)
            : base(sizeLimit)
        {
            Course = course ?? throw new ArgumentNullException(nameof(course));
            if (sizeLimit < 0)
                throw new ArgumentException("Group size limit cannot be negative", nameof(sizeLimit));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public ExtraCourse Course { get; }
        public string Name { get; }

        public override void AddStudent(Student student)
        {
            if (student is null)
                throw new ArgumentNullException(nameof(student));

            if (Regex.IsMatch(student.Group.GroupName.StringName, Course.MegaFaculty.GroupNamePattern))
                throw new IsuException("Student cannot be applied to the extra course of its mega faculty");

            ImmutableList<ExtraGroup> studentsGroups = Course.ExtraIsuServiceImpl.GetExtraGroupsOfStudent(student);
            if (studentsGroups.Contains(this))
                throw new IsuException("Student is already applied to this group");
            if (studentsGroups.Count >= Course.ExtraIsuServiceImpl.MaxExtraCoursesCount)
            {
                throw new IsuException(
                    $"Student cannot be applied to more than {Course.ExtraIsuServiceImpl.MaxExtraCoursesCount} extra courses");
            }

            ImmutableList<Classes> groupClasses = GetClasses();
            IEnumerable<Classes> studentClasses = studentsGroups.SelectMany(group => group.GetClasses()).Concat(student.Group.GetClasses());
            if (!studentClasses.All(classes => groupClasses.All(classes.NotIntersecting)))
                throw new IsuException("Student has intersecting classes with this group");
            base.AddStudent(student);
        }

        public override string GetName()
        {
            return Name;
        }
    }
}