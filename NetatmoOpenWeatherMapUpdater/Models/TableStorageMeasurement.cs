using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetatmoOpenWeatherMapUpdater
{
    public class TableStorageMeasurement : TableEntity
    {
        public string StatioName { get; set; }
        public string ModuleName { get; set; }
        public string ModuleType { get; set; }
        public long LastMessage { get; set; }
        public long LastSeen { get; set; }
        public long LastSetup { get; set; }
        public long LastUpgrade { get; set; }
        public long UnixTimestamp { get; set; }
        public double Temperature { get; set; }
        public string TemperatureTrend { get; set; }
        public double MinTemperature { get; set; }
        public long MinTemperatureUnixTimestamp { get; set; }
        public double MaxTemperature { get; set; }
        public long MaxTemperatureUnixTimestamp { get; set; }
        public long Humidity { get; set; }
        public long? CO2 { get; set; }
        public double? AbsolutePressure { get; set; }
        public double? Pressure { get; set; }
        public string PressureTrend { get; set; }
        public long? Noise { get; set; }
        public long BatteryPercentage { get; set; }
        public long BatteryVp { get; set; }
        public long RfStatus { get; set; }
        public long WifiStatus { get; set; }
        public long Firmware { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public long? Altitude { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Timezone { get; set; }
    }
}
