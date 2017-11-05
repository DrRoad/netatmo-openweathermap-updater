namespace NetatmoOpenWeatherMapUpdater
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class NetatmoStationData
    {
        [JsonProperty("body")]
        public Body Body { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("time_exec")]
        public double TimeExec { get; set; }

        [JsonProperty("time_server")]
        public long TimeServer { get; set; }
    }

    public partial class Body
    {
        [JsonProperty("devices")]
        public List<Device> Devices { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }

    public partial class User
    {
        [JsonProperty("administrative")]
        public Administrative Administrative { get; set; }

        [JsonProperty("mail")]
        public string Mail { get; set; }
    }

    public partial class Administrative
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("feel_like_algo")]
        public long FeelLikeAlgo { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("pressureunit")]
        public long Pressureunit { get; set; }

        [JsonProperty("reg_locale")]
        public string RegLocale { get; set; }

        [JsonProperty("unit")]
        public long Unit { get; set; }

        [JsonProperty("windunit")]
        public long Windunit { get; set; }
    }

    public partial class Device
    {
        [JsonProperty("cipher_id")]
        public string CipherId { get; set; }

        [JsonProperty("co2_calibrating")]
        public bool Co2Calibrating { get; set; }

        [JsonProperty("dashboard_data")]
        public PurpleDashboardData DashboardData { get; set; }

        [JsonProperty("data_type")]
        public List<string> DataType { get; set; }

        [JsonProperty("date_setup")]
        public long DateSetup { get; set; }

        [JsonProperty("firmware")]
        public long Firmware { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("last_setup")]
        public long LastSetup { get; set; }

        [JsonProperty("last_status_store")]
        public long LastStatusStore { get; set; }

        [JsonProperty("last_upgrade")]
        public long LastUpgrade { get; set; }

        [JsonProperty("module_name")]
        public string ModuleName { get; set; }

        [JsonProperty("modules")]
        public List<Module> Modules { get; set; }

        [JsonProperty("place")]
        public Place Place { get; set; }

        [JsonProperty("station_name")]
        public string StationName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("wifi_status")]
        public long WifiStatus { get; set; }
    }

    public partial class Place
    {
        [JsonProperty("altitude")]
        public long Altitude { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("location")]
        public List<double> Location { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }
    }

    public partial class Module
    {
        [JsonProperty("battery_percent")]
        public long BatteryPercent { get; set; }

        [JsonProperty("battery_vp")]
        public long BatteryVp { get; set; }

        [JsonProperty("dashboard_data")]
        public FluffyDashboardData DashboardData { get; set; }

        [JsonProperty("data_type")]
        public List<string> DataType { get; set; }

        [JsonProperty("firmware")]
        public long Firmware { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("last_message")]
        public long LastMessage { get; set; }

        [JsonProperty("last_seen")]
        public long LastSeen { get; set; }

        [JsonProperty("last_setup")]
        public long LastSetup { get; set; }

        [JsonProperty("module_name")]
        public string ModuleName { get; set; }

        [JsonProperty("rf_status")]
        public long RfStatus { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class FluffyDashboardData
    {
        [JsonProperty("CO2")]
        public long? CO2 { get; set; }

        [JsonProperty("date_max_temp")]
        public long DateMaxTemp { get; set; }

        [JsonProperty("date_min_temp")]
        public long DateMinTemp { get; set; }

        [JsonProperty("Humidity")]
        public long Humidity { get; set; }

        [JsonProperty("max_temp")]
        public double MaxTemp { get; set; }

        [JsonProperty("min_temp")]
        public double MinTemp { get; set; }

        [JsonProperty("temp_trend")]
        public string TempTrend { get; set; }

        [JsonProperty("Temperature")]
        public double Temperature { get; set; }

        [JsonProperty("time_utc")]
        public long TimeUtc { get; set; }
    }

    public partial class PurpleDashboardData
    {
        [JsonProperty("AbsolutePressure")]
        public double AbsolutePressure { get; set; }

        [JsonProperty("CO2")]
        public long CO2 { get; set; }

        [JsonProperty("date_max_temp")]
        public long DateMaxTemp { get; set; }

        [JsonProperty("date_min_temp")]
        public long DateMinTemp { get; set; }

        [JsonProperty("Humidity")]
        public long Humidity { get; set; }

        [JsonProperty("max_temp")]
        public long MaxTemp { get; set; }

        [JsonProperty("min_temp")]
        public long MinTemp { get; set; }

        [JsonProperty("Noise")]
        public long Noise { get; set; }

        [JsonProperty("Pressure")]
        public long Pressure { get; set; }

        [JsonProperty("pressure_trend")]
        public string PressureTrend { get; set; }

        [JsonProperty("temp_trend")]
        public string TempTrend { get; set; }

        [JsonProperty("Temperature")]
        public double Temperature { get; set; }

        [JsonProperty("time_utc")]
        public long TimeUtc { get; set; }
    }

    public partial class NetatmoStationData
    {
        public static NetatmoStationData FromJson(string json) => JsonConvert.DeserializeObject<NetatmoStationData>(json, NetatmoStationDataConverter.Settings);
    }

    public static class SerializeNetatmoStationData
    {
        public static string ToJson(this NetatmoStationData self) => JsonConvert.SerializeObject(self, NetatmoStationDataConverter.Settings);
    }

    public class NetatmoStationDataConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
