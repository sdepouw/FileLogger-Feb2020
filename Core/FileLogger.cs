using System;

namespace FileIO.Core
{
    public class FileLogger
    {
        private readonly IDateTime _dateTime;
        private readonly IFileSystem _fileSystem;

        public FileLogger()
            : this(new SystemDateTime(), new FileSystem())
        { }

        public FileLogger(IDateTime dateTime, IFileSystem fileSystem)
        {
            _dateTime = dateTime;
            _fileSystem = fileSystem;
        }

        public void Log(string message)
        {
            string fileName;
            if (_dateTime.Now.IsWeekend())
            {
                fileName = GetWeekendFileName();
                ArchivePreviousWeekendLog(fileName);
            }
            else
            {
                fileName = GetWeekdayFileName(_dateTime.Now);
            }

            _fileSystem.AppendAllText(fileName, message);
        }

        private void ArchivePreviousWeekendLog(string weekendFileName)
        {
            if (!_fileSystem.Exists(weekendFileName)) return;

            DateTime lastModified = _fileSystem.GetLastWriteTime(weekendFileName);
            DateTime currentWeekendStartDate = _dateTime.Now.GetPreviousSaturday().Date;
            bool lastWrittenOnEarlierWeekend = lastModified < currentWeekendStartDate;
            if (lastWrittenOnEarlierWeekend)
            {
                DateTime saturdayOfLastWrittenWekeend = lastModified.GetPreviousSaturday();
                _fileSystem.Move(weekendFileName, GetWeekendFileName(saturdayOfLastWrittenWekeend));
            }
        }

        public static string GetWeekdayFileName(DateTime date) => $"log{date.GetLogFormat()}.txt";
        public static string GetWeekendFileName() => "weekend.txt";
        public static string GetWeekendFileName(DateTime date) => $"weekend-{date.GetLogFormat()}.txt";
    }
}
