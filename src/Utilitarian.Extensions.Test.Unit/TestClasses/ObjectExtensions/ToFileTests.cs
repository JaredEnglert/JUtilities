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
        public void ToFile_ShouldWriteFileFromObject()
        {
            1.ToFile(FilePath);

            var fileInfo = new FileInfo(FilePath);

            fileInfo.Exists.Should().BeTrue();

            fileInfo.Delete();
        }

        [TestMethod]
        public void ToFile_DirectoryDoesNotExist_ShouldCreateDirectory()
        {
            var directory = Guid.NewGuid().ToString();
            var filePath = $"{directory}\\{FilePath}";

            DeleteDirectoryIfExists(directory);

            1.ToFile(filePath);

            new DirectoryInfo(directory).Exists.Should().BeTrue();
            
            DeleteDirectoryIfExists(directory);
        }

        private static void DeleteDirectoryIfExists(string filePath)
        {
            var directoryInfo = new DirectoryInfo(filePath);

            if (directoryInfo.Exists)
            {
                directoryInfo.Delete(true);
            }
        }
    }
}
