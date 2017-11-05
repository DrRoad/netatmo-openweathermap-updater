using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetatmoOpenWeatherMapUpdater.Helpers
{
    public static class TimestampExtensions
    {
        public static DateTime FromUnixTimestampToDateTime(this long self) => DateTimeOffset.FromUnixTimeSeconds(self).UtcDateTime;
    }
}

