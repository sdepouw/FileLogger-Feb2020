using System;
using FileIO.Core;
using Xunit;

namespace FileIO.Tests.DateTimeExtensionsTests
{
    public class GetPreviousSaturday
    {
        [Fact]
        public void ReturnsTodayWhenTodayIsSaturday()
        {
            DateTime saturday = new DateTime(2020, 2, 22);

            DateTime previousSaturday = saturday.GetPreviousSaturday();

            Assert.Equal(saturday, previousSaturday);
        }

        [Theory]
        [InlineData(21)] // Friday
        [InlineData(20)] // Thursday
        [InlineData(19)] // Wednesday
        [InlineData(18)] // Tuesday
        [InlineData(17)] // Monday
        [InlineData(16)] // Sunday
        public void ReturnsNearestSaturdayGoingBackwards(int day)
        {
            DateTime expectedSaturday = new DateTime(2020, 2, 15);
            DateTime today = new DateTime(2020, 2, day);

            DateTime previousSaturday = today.GetPreviousSaturday();

            Assert.Equal(expectedSaturday, previousSaturday);
        }
    }
}