using System;
using Isu.Entities;
using Isu.Services;
using Isu.Tools;
using NUnit.Framework;

namespace Isu.Tests
{ 
    [TestFixture]
    public class Tests
    {
        private IIsuService _isuService;
        private Group _m3200;

        [SetUp]
        public void Setup()
        {
            _isuService = new IsuServiceImpl();
            _m3200 = _isuService.AddGroup(new GroupName("M3200"));
        }

        [Test]
        public void CreateGroupName()
        {
            Assert.Catch<IsuException>(() =>
            {
                var groupName = new GroupName("123");
            });
            Assert.Catch<IsuException>(() =>
            {
                var groupName = new GroupName("M31da");
            });
            Assert.Catch<IsuException>(() =>
            {
                var groupName = new GroupName("NnnN");
            });
            Assert.Catch<IsuException>(() =>
            {
                var groupName = new GroupName("M3ddd");
            });
            try
            {
                var groupName = new GroupName("M3101");
                groupName = new GroupName("M3200");
            }
            catch (IsuException e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent()
        {
            Assert.Catch<IsuException>(() =>
            {
                _isuService.AddStudent(null, null);
            });
            
            
            var student = _isuService.AddStudent(_m3200, "Name");
            Assert.True(_m3200.Students.Contains(student));
            Assert.AreEqual(student.Group, _m3200);
        }

        [Test]
        public void ReachMaxStudentPerGroup_ThrowException()
        {
            Group testGroup = _isuService.AddGroup(new GroupName("M3311"));
            Assert.Catch<IsuException>(() =>
            {
                for (int i = 0; i <= IsuServiceImpl.MaxStudentsInGroupCount; i++)
                {
                    _isuService.AddStudent(testGroup, "Student" + i);
                }
            });
        }

        [Test]
        public void CreateGroupWithInvalidName_ThrowException()
        {
            Assert.Catch<IsuException>(() =>
            {
                _isuService.AddGroup(new GroupName("a1233"));
            });
        }

        [Test]
        public void TransferStudentToAnotherGroup_GroupChanged()
        {
            Student testStudent = _isuService.AddStudent(_m3200, "transferTestStudent");
            Group testGroup = _isuService.AddGroup(new GroupName("M3214"));
            
            _m3200.AddStudent(testStudent);
            Assert.True(_m3200.Students.Contains(testStudent));
            Assert.True(!testGroup.Students.Contains(testStudent));
            
            _isuService.ChangeStudentGroup(testStudent, testGroup);
            Assert.True(!_m3200.Students.Contains(testStudent));
            Assert.True(testGroup.Students.Contains(testStudent));
        }
    }
}