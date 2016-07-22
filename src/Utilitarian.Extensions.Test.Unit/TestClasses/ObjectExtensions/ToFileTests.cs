using System;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilitarian.Extensions.Test.Unit.TestClasses.ObjectExtensions
{
    [TestClass]
    public class ToFileTests
    {
        private const string FilePath = "TestFile.dummy";

        [TestMethod]
        public void ShouldWriteFileFromObject()
        {
            1.ToFile(FilePath);

            var fileInfo = new FileInfo(FilePath);

            fileInfo.Exists.Should().BeTrue();

            fileInfo.Delete();
        }

        [TestMethod]
        public void ShouldCreateDirectoryIfDoesNotExist()
        {
            var directory = Guid.NewGuid().ToString();
            var filePath = string.Format("{0}\\{1}", directory, FilePath);

            DeleteDirectoryIfExists(directory);

            1.ToFile(filePath);
            
            DeleteDirectoryIfExists(directory);
        }

        private void DeleteDirectoryIfExists(string filePath)
        {
            var directoryInfo = new DirectoryInfo(filePath);

            if (directoryInfo.Exists)
            {
                directoryInfo.Delete(true);
            }
        }
    }
}
