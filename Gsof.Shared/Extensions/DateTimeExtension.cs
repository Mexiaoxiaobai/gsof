using System;
using System.Collections.Generic;
using System.Text;

namespace Gsof.Shared.Extensions
{
    public static class DateTimeExtension
    {
        public static long ToTimestamp(this DateTime p_dateTime)
        {
            return (long)(p_dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
        }

        public static DateTime ToDateTime(this long p_timestamp)
        {
            var ts = TimeSpan.FromMilliseconds(p_timestamp);
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).Add(ts);
        }
    }
}
