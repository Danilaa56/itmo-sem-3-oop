using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Isu.Entities;
using Isu.Services;
using Isu.Tools;
using IsuExtra.Entities;

namespace IsuExtra.Services
{
    public class ExtraIsuServiceImpl : IsuServiceImpl
    {
        private readonly Dictionary<string, MegaFaculty> _megaFaculties = new Dictionary<string, MegaFaculty>();

        private readonly Dictionary<MegaFaculty, List<ExtraCourse>> _extraCourses =
            new Dictionary<MegaFaculty, List<ExtraCourse>>();

        private Dictionary<Student, List<ExtraCourse>> _studentExtraCourses =
            new Dictionary<Student, List<ExtraCourse>>();

        private int _maxExtraCoursesCount = 2;

        public int MaxExtraCoursesCount
        {
            get => _maxExtraCoursesCount;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Count of extra courses cannot be negative", nameof(value));
                _maxExtraCoursesCount = value;
            }
        }

        public MegaFaculty AddMegaFaculty(string name, string groupNamePattern)
        {
            if (_megaFaculties.ContainsKey(name))
                throw new IsuException("Mega faculty with such name already exists");

            var megaFaculty = new MegaFaculty(name, groupNamePattern);
            _megaFaculties[name] = megaFaculty;
            _extraCourses[megaFaculty] = new List<ExtraCourse>();
            return megaFaculty;
        }

        public void DestroyMegaFaculty(string name)
        {
            if (!_megaFaculties.TryGetValue(name, out MegaFaculty megaFaculty))
            {
                throw new IsuException("There is no mega faculty with such name");
            }

            _megaFaculties.Remove(name);
            _extraCourses.Remove(megaFaculty);
        }

        public ExtraCourse AddExtraCourse(MegaFaculty megaFaculty, string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (megaFaculty is null)
                throw new ArgumentNullException(nameof(megaFaculty));

            if (!_megaFaculties.TryGetValue(megaFaculty.Name, out MegaFaculty existing) || megaFaculty != existing)
            {
                throw new ArgumentException("This mega faculty does not apply to this IsuService", nameof(megaFaculty));
            }

            List<ExtraCourse> megaFacultyCourses = _extraCourses[megaFaculty];

            if (megaFacultyCourses.Find(extraCourse => extraCourse.Name.Equals(name)) != null)
                throw new IsuException("There is another extra course with such name on this mega faculty");

            var extraCourse = new ExtraCourse(this, megaFaculty, name);
            megaFacultyCourses.Add(extraCourse);

            return extraCourse;
        }

        public ImmutableList<ExtraCourse> GetExtraCourses()
        {
            return _extraCourses.Values.SelectMany(courses => courses).ToImmutableList();
        }

        public ImmutableList<Student> GetStudentsWithoutExtraCourses(Group group)
        {
            if (group is null)
                throw new ArgumentNullException(nameof(group));
            var appointedStudents = _extraCourses.Values
                .SelectMany(courses => courses)
                .SelectMany(course => course.GetGroups())
                .SelectMany(group => group.GetStudents())
                .ToImmutableHashSet();
            return group.GetStudents().Where(student => !appointedStudents.Contains(student)).ToImmutableList();
        }

        public ImmutableList<ExtraGroup> GetExtraGroupsOfStudent(Student student)
        {
            if (student is null)
                throw new ArgumentNullException(nameof(student));
            return _extraCourses.Values.SelectMany(courses => courses)
                .SelectMany(course => course.GetGroups())
                .Where(group => group.GetStudents().Contains(student)).ToImmutableList();
        }
    }
}