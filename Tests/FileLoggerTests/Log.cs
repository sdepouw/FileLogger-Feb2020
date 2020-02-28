using System;
using FileIO.Core;
using Moq;
using Xunit;

namespace FileIO.Tests.FileLoggerTests
{
    public class Log
    {
        private readonly FileLogger _fileLogger;
        private readonly Mock<IDateTime> _mockDateTime = new Mock<IDateTime>();
        private readonly Mock<IFileSystem> _mockFileSystem = new Mock<IFileSystem>();

        private static readonly DateTime _monday = new DateTime(2020, 2, 3, 11, 50, 23);
        private static readonly DateTime _saturday = new DateTime(2019, 11, 2, 18, 30, 05);

        public Log()
        {
            _fileLogger = new FileLogger(_mockDateTime.Object, _mockFileSystem.Object);
        }

        [Fact]
        public void CreatesFileWithCurrentDateAndLogs()
        {
            _mockDateTime.SetupGet(date => date.Now).Returns(_monday);
            string expectedFileName = FileLogger.GetWeekdayFileName(_monday);
            const string expectedMessage = "Some text";
            _fileLogger.Log(expectedMessage);

            _mockFileSystem.Verify(fs => fs.AppendAllText(expectedFileName, expectedMessage), Times.Once);
        }

        [Fact]
        public void AppendsToExistingLog()
        {
            _mockDateTime.SetupGet(date => date.Now).Returns(_monday);
            string expectedFileName = FileLogger.GetWeekdayFileName(_monday);

            _fileLogger.Log("foo");
            _fileLogger.Log("bar");

            _mockFileSystem.Verify(fs => fs.AppendAllText(expectedFileName, It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void CreateNewFileForNewDate()
        {
            _mockDateTime.SetupGet(date => date.Now).Returns(_monday);
            const string firstMessage = "Some Text";
            _fileLogger.Log(firstMessage);
            string firstFile = FileLogger.GetWeekdayFileName(_monday);

            DateTime tuesday = _monday.AddDays(1);
            _mockDateTime.SetupGet(date => date.Now).Returns(tuesday);
            string secondFile = FileLogger.GetWeekdayFileName(tuesday);
            const string secondMessage = "expected log message";

            _fileLogger.Log(secondMessage);

            _mockFileSystem.Verify(fs => fs.AppendAllText(firstFile, firstMessage), Times.Once);
            _mockFileSystem.Verify(fs => fs.AppendAllText(secondFile, secondMessage), Times.Once);
        }

        [Fact]
        public void LogWeekendsToSingleFile()
        {
            _mockDateTime.SetupGet(date => date.Now).Returns(_saturday);

            _fileLogger.Log("first");
            _fileLogger.Log("second");

            _mockFileSystem.Verify(fs => fs.AppendAllText(FileLogger.GetWeekendFileName(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void RenameOldWeekendLogOnNewWeekend()
        {
            string weekendFileName = FileLogger.GetWeekendFileName();
            _mockFileSystem.Setup(fs => fs.Exists(weekendFileName)).Returns(true); // Pretend we logged stuff already.
            _mockFileSystem.Setup(fs => fs.GetLastWriteTime(weekendFileName)).Returns(_saturday.AddDays(1));
            DateTime nextSaturday = _saturday.AddDays(7);
            _mockDateTime.SetupGet(date => date.Now).Returns(nextSaturday);

            const string newWeekendMessage = "bar";
            _fileLogger.Log(newWeekendMessage);

            _mockFileSystem.Verify(fs => fs.Move(weekendFileName, FileLogger.GetWeekendFileName(_saturday)), Times.Once);
            _mockFileSystem.Verify(fs => fs.AppendAllText(weekendFileName, newWeekendMessage), Times.Once);
        }

        [Fact]
        public void ContinueLoggingToCurrentWeekend()
        {
            string weekendFileName = FileLogger.GetWeekendFileName();
            const string sameWeekendMessage = "foobar";

            _mockFileSystem.Setup(fs => fs.Exists(weekendFileName)).Returns(true);
            _mockFileSystem.Setup(fs => fs.GetLastWriteTime(weekendFileName)).Returns(_saturday);
            _mockDateTime.SetupGet(date => date.Now).Returns(_saturday.AddDays(1));

            _fileLogger.Log(sameWeekendMessage);

            _mockFileSystem.Verify(fs => fs.Move(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockFileSystem.Verify(fs => fs.AppendAllText(weekendFileName, sameWeekendMessage), Times.Once);
        }
    }
}
