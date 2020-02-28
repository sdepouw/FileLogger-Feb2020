using System;
using FileIO.Core;
using Xunit;

namespace FileIO.Tests.DateTimeExtensionsTests
{
    public class IsWeekend
    {
        [Theory]
        [InlineData(DayOfWeek.Monday)]
        [InlineData(DayOfWeek.Tuesday)]
        [InlineData(DayOfWeek.Wednesday)]
        [InlineData(DayOfWeek.Thursday)]
        [InlineData(DayOfWeek.Friday)]
        public void IsFalseForWeekdays(DayOfWeek dayOfWeek)
        {
            DateTime date = GetDateForDayOfWeek(dayOfWeek);

            Assert.False(date.IsWeekend());
        }

        [Theory]
        [InlineData(DayOfWeek.Saturday)]
        [InlineData(DayOfWeek.Sunday)]
        public void IsFalseForWeekendDays(DayOfWeek dayOfWeek)
        {
            DateTime date = GetDateForDayOfWeek(dayOfWeek);

            Assert.True(date.IsWeekend());
        }

        private DateTime GetDateForDayOfWeek(DayOfWeek dayOfWeek)
        {
            DateTime date = DateTime.Today;
            while (date.DayOfWeek != dayOfWeek)
            {
                date = date.AddDays(1);
            }
            return date;
        }
    }
}