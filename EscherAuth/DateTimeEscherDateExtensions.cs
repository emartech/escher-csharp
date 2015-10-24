using System;

namespace EscherAuth
{
    static class DateTimeEscherDateExtensions
    {
        public static string ToEscherLongDate(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMddTHHmmssZ");
        }

        public static string ToEscherShortDate(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }
    }
}
