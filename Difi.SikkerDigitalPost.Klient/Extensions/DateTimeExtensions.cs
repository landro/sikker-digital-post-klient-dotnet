using System;

namespace Difi.SikkerDigitalPost.Klient.Extensions
{
    internal static class DateTimeExtensions
    {
        public static string ToStringWithUtcOffset(this DateTime dateTime)
        {
            const string fullFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff";

            string date = dateTime.ToString(fullFormat);

            TimeZoneInfo timeZone = TimeZoneInfo.Local;
            TimeSpan offset = timeZone.GetUtcOffset(dateTime);
          
            var fortegn = offset.CompareTo(TimeSpan.Zero) >= 0 ? "'+'" : "'-'";
            
            return dateTime.ToString(String.Format("{0}{1}{2}:{3}", date, fortegn, offset.Hours.ToString("hh"), offset.Minutes.ToString("mm") ));
        }

    }
}
