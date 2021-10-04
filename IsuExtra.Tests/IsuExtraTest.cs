using System.Linq;
using Isu.Entities;
using IsuExtra.Entities;
using IsuExtra.Services;
using NUnit.Framework;

namespace IsuExtra.Tests
{
    [TestFixture]
    public class Tests
    {
        private ExtraIsuServiceImpl _isuIsuServiceImpl;
        private MegaFaculty _tint;
        private Group _m3101, _m3200, _o4144;

        [SetUp]
        public void Setup()
        {
            _isuIsuServiceImpl = new ExtraIsuServiceImpl();
            _tint = _isuIsuServiceImpl.AddMegaFaculty("ТИнТ", "^[CJKM][34][1-4][0-9]{2,3}[c]*$");

            _m3101 = _isuIsuServiceImpl.AddGroup(new GroupName("M3101"));
            _m3200 = _isuIsuServiceImpl.AddGroup(new GroupName("M3200"));
            _o4144 = _isuIsuServiceImpl.AddGroup(new GroupName("O4144"));
        }

        [Test]
        public void CreateExtraCourseTest()
        {
            string courseName = "CompSec";
            Assert.False(_isuIsuServiceImpl.GetExtraCourses().Any(course => course.Name.Equals(courseName)));
            ExtraCourse extraCourse = _isuIsuServiceImpl.AddExtraCourse(_tint, courseName);
            Assert.True(_isuIsuServiceImpl.GetExtraCourses().Any(course => course.Name.Equals(courseName)));
        }

        [Test]
        public void ApplyStudentTest()
        {
            ExtraCourse extraCourse = _isuIsuServiceImpl.AddExtraCourse(_tint, "ИнфоБез");
            ExtraGroup extraGroup = extraCourse.AddGroup("КИБ 1", 10);
            Student student = _isuIsuServiceImpl.AddStudent(_o4144, "Валера");
            
            Assert.True(_isuIsuServiceImpl.GetStudentsWithoutExtraCourses(_o4144).Contains(student));
            extraGroup.AddStudent(student);
            Assert.False(_isuIsuServiceImpl.GetStudentsWithoutExtraCourses(_o4144).Contains(student));
        }

        [Test]
        public void UnapplyStudentTest()
        {
            ExtraCourse extraCourse = _isuIsuServiceImpl.AddExtraCourse(_tint, "ИнфоБез");
            ExtraGroup extraGroup = extraCourse.AddGroup("КИБ 1", 10);
            Student student = _isuIsuServiceImpl.AddStudent(_o4144, "Валера");

            extraGroup.AddStudent(student);
            Assert.False(_isuIsuServiceImpl.GetStudentsWithoutExtraCourses(_o4144).Contains(student));
            extraGroup.KickStudent(student);
            Assert.True(_isuIsuServiceImpl.GetStudentsWithoutExtraCourses(_o4144).Contains(student));
        }
        
        [Test]
        public void GetStudents()
        {
            ExtraCourse extraCourse = _isuIsuServiceImpl.AddExtraCourse(_tint, "ИнфоБез");
            ExtraGroup extraGroup = extraCourse.AddGroup("КИБ 1", 10);
            Student student = _isuIsuServiceImpl.AddStudent(_o4144, "Валера");
            
            Assert.True(_isuIsuServiceImpl.GetStudentsWithoutExtraCourses(_o4144).Contains(student));
            extraGroup.AddStudent(student);
            Assert.False(_isuIsuServiceImpl.GetStudentsWithoutExtraCourses(_o4144).Contains(student));
        }   
    }
}