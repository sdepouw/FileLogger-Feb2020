using System;
using System.IO;
using FileIO.Core;
using Moq;
using Xunit;

namespace FileIO.Tests.FileLoggerTests
{
    [Trait("Category", "Integration")]
    public class Integration : IDisposable
    {
        // Call clearer here in case Dispose() wasn't called. Maybe just clear here only? Makes more sense to happen in Dispose()...
        public Integration() => ClearLogFiles();

        // Runs at the end of each test (XUnit convention). Note the IDisposable interface.
        // Does not get called if test hangs or is canceled!
        public void Dispose() => ClearLogFiles();

        private void ClearLogFiles()
        {
            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory());
            var files = di.GetFiles("*.txt");
            foreach (var file in files)
            {
                File.Delete(file.FullName);
            }
        }

        [Fact]
        public void FullSystemTest()
        {
            // Still mock DateTime so we can simulate different days.
            // We also manually set "last modified" times throughout the test to correspond with mocked dates.
            Mock<IDateTime> mockDateTime = new Mock<IDateTime>();
            FileLogger logger = new FileLogger(mockDateTime.Object, new FileSystem());
            DateTime monday = new DateTime(2020, 2, 17, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            mockDateTime.SetupGet(date => date.Now).Returns(monday);

            // Verify weekday logging.
            logger.Log("Foo");
            logger.Log("Bar");

            string mondayFileName = FileLogger.GetWeekdayFileName(monday);
            Assert.True(File.Exists(mondayFileName));
            File.SetLastWriteTime(mondayFileName, monday);
            Assert.Contains("Foo", File.ReadAllText(mondayFileName));
            Assert.Contains("Bar", File.ReadAllText(mondayFileName));

            // Verify weekend logging.
            DateTime saturday = monday.AddDays(5);
            mockDateTime.SetupGet(date => date.Now).Returns(saturday);
            logger.Log("Bizz");

            DateTime sunday = saturday.AddDays(1);
            mockDateTime.SetupGet(date => date.Now).Returns(sunday);
            logger.Log("Buzz");

            string weekendFileName = FileLogger.GetWeekendFileName();
            Assert.True(File.Exists(weekendFileName));
            File.SetLastWriteTime(weekendFileName, sunday);
            Assert.Contains("Bizz", File.ReadAllText(weekendFileName));
            Assert.Contains("Buzz", File.ReadAllText(weekendFileName));

            // Verify previous weekend archiving.
            string previousWeekendFileName = FileLogger.GetWeekendFileName(saturday);
            DateTime nextSunday = sunday.AddDays(7);
            mockDateTime.SetupGet(date => date.Now).Returns(nextSunday);
            logger.Log("Fizzy");

            Assert.True(File.Exists(previousWeekendFileName));
            File.SetLastWriteTime(previousWeekendFileName, nextSunday);
            Assert.True(File.Exists(weekendFileName));
            File.SetLastWriteTime(weekendFileName, nextSunday);

            logger.Log("Pop");

            // Previous weekend should contain Bizz and Buzz.
            Assert.Contains("Bizz", File.ReadAllText(previousWeekendFileName));
            Assert.Contains("Buzz", File.ReadAllText(previousWeekendFileName));

            // Current weekend gets the new Fizzy Pop messages.
            Assert.True(File.Exists(weekendFileName));
            Assert.Contains("Fizzy", File.ReadAllText(weekendFileName));
            Assert.Contains("Pop", File.ReadAllText(weekendFileName));
        }

        [Fact]
        public void TestTodayWithNoMocking()
        {
            FileLogger fileLogger = new FileLogger();

            fileLogger.Log("Foo");
            string fileName = DateTime.Now.IsWeekend() ? FileLogger.GetWeekendFileName() : FileLogger.GetWeekdayFileName(DateTime.Now);

            Assert.True(File.Exists(fileName));
            Assert.Contains("Foo", File.ReadAllText(fileName));
        }
    }
}