using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Directory.Monitor.Core;

namespace Services.Directory.Monitor.Tests.Services.Directory.Monitor.Core
{
    [TestClass]
    public class MkvConvertorTests
    {
        private readonly string _filePath = System.IO.Directory.GetCurrentDirectory() + @"\MkvFiles\";

        [TestMethod]
        public void MkvConvertor_ValidFilePath_SuccessObjectCreated() 
        { 
            // Arrange
            var file = _filePath + "test1.mkv";

            // Act
            var mkvConvertor = new MkvConvertor(file);

            // Assert
            Assert.IsNotNull(mkvConvertor);
        }

        [TestMethod]
        public void Convert_ValidDestinationPath_SuccessDestinationFileCreated()
        {
            // Arrange
            var file = _filePath + "test1.mkv";
            var destinationFile = file + ".mp4";
            var mkvConvertor = new MkvConvertor(file);

            // Act
            mkvConvertor.Convert(destinationFile);

            // Assert
            Assert.IsNotNull(mkvConvertor);
            Assert.IsTrue(System.IO.File.Exists(destinationFile));
        }

        [TestMethod]
        public void Convert_ValidDestinationPathFileLocked_FailureNoFileCreated()
        {
            // Arrange
            var file = _filePath + "test1.mkv";
            var destinationFile = file + ".mp4";
            var mkvConvertor1 = new MkvConvertor(file);

            // Act
            mkvConvertor1.Convert(destinationFile);
            mkvConvertor1.Convert(destinationFile);

            // Assert
            Assert.IsNotNull(mkvConvertor1);
            Assert.IsFalse(System.IO.File.Exists(destinationFile));
        } 
    }
}
