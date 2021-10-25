using System;
using System.IO;
using System.Linq;
using Backups.Entities;
using NUnit.Framework;

namespace Backups.Tests
{ 
    [TestFixture]
    public class Tests
    {
        /*
1. Тест-1
    1. Cоздаю бекапную джобу
    2. Указываю Split storages
    3. Добавляю в джобу два файла
    4. Запускаю создание точки 
    5. Удаляю один из файлов
    6. Запускаю создание
    7. Проверяю, что создано две точки и три стораджа
         */

        // [SetUp]
        // public void Setup()
        // {
        //     _isuService = new IsuServiceImpl();
        //     _m3200 = _isuService.AddGroup(new GroupName("M3200"));
        // }

        // private static void WriteFile(string fileName, string data)
        // {
        //     File.WriteAllText(fileName, data);
        // }
        
        [Test]
        public void Test1()
        {
            Directory.CreateDirectory("tmp\\data");
            Directory.CreateDirectory("tmp\\repo");

            File.WriteAllText("tmp\\data\\file1.txt", "Hello 1");
            File.WriteAllText("tmp\\data\\file2.txt", "Hello 2 and longer");

            Repository repo = new RepositoryLocal("tmp\\repo");
            
            var backupJob = new BackupJob(repo, StorageType.SplitStorages);
            var jobObject1 = new JobObject("tmp\\data", "file1.txt");
            var jobObject2 = new JobObject("tmp\\data", "file2.txt");
            
            backupJob.Add(jobObject1);
            backupJob.Add(jobObject2);
            var restorePoint1 = backupJob.CreateRestorePoint();
            
            backupJob.Remove(jobObject1);
            var restorePoint2 = backupJob.CreateRestorePoint();

            Assert.AreEqual(2, backupJob.GetRestorePoints().Count);
            Assert.AreEqual(3, new DirectoryInfo("tmp\\repo").EnumerateFiles().Count());

            DeleteDirRecursively(new DirectoryInfo("tmp"));
        }

        private static void DeleteDirRecursively(DirectoryInfo dir)
        {
            foreach (var enumerateFileSystemInfo in dir.EnumerateFileSystemInfos())
            {
                if(enumerateFileSystemInfo is DirectoryInfo)
                    DeleteDirRecursively((DirectoryInfo) enumerateFileSystemInfo);
                else
                    enumerateFileSystemInfo.Delete();
            }
            dir.Delete();
        }
/*
2. Тест-2, который лучше оформлять не тестом т.к. посмотреть нормально можно только на настоящей файловой системе
    1. Cоздаю бекапную джобу, указываю путь директории для хранения бекапов
    2. Указываю Single storage
    3. Добавляю в джобу два файла
    4. Запускаю создание точки
    5. Проверяю, что созданы директории и файлы
    */
        // [Test]
        // public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent()
        // {
        //     Student student = _isuService.AddStudent(_m3200, "Name");
        //     Assert.True(_m3200.GetStudents().Contains(student));
        //     Assert.AreEqual(student.Group, _m3200);
        // }
        //
        // [Test]
        // public void ReachMaxStudentPerGroup_ThrowException()
        // {
        //     Group testGroup = _isuService.AddGroup(new GroupName("M3311"));
        //     for (int i = 0; i < IsuServiceImpl.MaxStudentsInGroupCount; i++)
        //     {
        //         _isuService.AddStudent(testGroup, "Student" + i);
        //     }
        //     Assert.Catch<IsuException>(() =>
        //     {
        //         _isuService.AddStudent(testGroup, "Student" + IsuServiceImpl.MaxStudentsInGroupCount);
        //     });
        // }
        //
        // [Test]
        // public void CreateGroupWithInvalidName_ThrowException()
        // {
        //     Assert.Catch<IsuException>(() =>
        //     {
        //         _isuService.AddGroup(new GroupName("a1233"));
        //     });
        // }
        //
        // [Test]
        // public void TransferStudentToAnotherGroup_GroupChanged()
        // {
        //     Student testStudent = _isuService.AddStudent(_m3200, "transferTestStudent");
        //     Group testGroup = _isuService.AddGroup(new GroupName("M3214"));
        //     
        //     Assert.True(_m3200.GetStudents().Contains(testStudent));
        //     Assert.True(!testGroup.GetStudents().Contains(testStudent));
        //     
        //     _isuService.ChangeStudentGroup(testStudent, testGroup);
        //     Assert.True(!_m3200.GetStudents().Contains(testStudent));
        //     Assert.True(testGroup.GetStudents().Contains(testStudent));
        // }
    }
}