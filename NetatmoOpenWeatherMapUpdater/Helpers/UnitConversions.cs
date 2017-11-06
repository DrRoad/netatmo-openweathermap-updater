using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetatmoOpenWeatherMapUpdater.Helpers
{
    public static class UnitConversions
    {
        public static double FromCelsiusToFahrenheit(double celsius)
        {
            return (9.0 * celsius) / 5.0 + 32;
        }

        public static double InHgFromMillibars(double millibars)
        {
            // https://www.weather.gov/media/epz/wxcalc/pressureConversion.pdf
            var mmHg = 0.750062 * millibars;
            return 0.03937008 * mmHg;
        }
    }
}
