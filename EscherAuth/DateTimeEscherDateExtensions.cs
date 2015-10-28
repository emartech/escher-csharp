using System;
using System.Globalization;

namespace EscherAuth
{
    static class DateTimeParser
    {
        private const string ShortDateFormat = "yyyyMMdd";
        private const string LongDateFormat = "yyyyMMddTHHmmssZ";

        public static string ToEscherLongDate(this DateTime dateTime)
        {
            return dateTime.ToString(LongDateFormat);
        }

        public static string ToEscherShortDate(this DateTime dateTime)
        {
            return dateTime.ToString(ShortDateFormat);
        }

        public static DateTime FromEscherShortDate(string shortDate)
        {
            return DateTime.ParseExact(shortDate, ShortDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
        }

        public static DateTime FromEscherLongDate(string longDate)
        {
            return DateTime.ParseExact(longDate, LongDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
        }
    }
}
