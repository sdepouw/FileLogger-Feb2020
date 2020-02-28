using System;

namespace FileIO.Core
{
    public static class DateTimeExtensions
    {
        public static string GetLogFormat(this DateTime dateTime) => dateTime.ToString("yyyyMMdd");
        public static bool IsWeekend(this DateTime dateTime) => dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;

        public static DateTime GetPreviousSaturday(this DateTime dateTime)
        {
            DateTime result = dateTime;
            while (result.DayOfWeek != DayOfWeek.Saturday)
            {
                result = result.AddDays(-1);
            }
            return result;
        }
    }
}
