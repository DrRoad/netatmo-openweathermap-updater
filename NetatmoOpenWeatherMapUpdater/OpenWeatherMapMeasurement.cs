namespace NetatmoOpenWeatherMapUpdater
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class OpenWeatherMapMeasurement
    {
        [JsonProperty("dt")]
        public long Dt { get; set; }

        [JsonProperty("humidity")]
        public long Humidity { get; set; }

        [JsonProperty("station_id")]
        public string StationId { get; set; }

        [JsonProperty("temperature")]
        public double Temperature { get; set; }
    }

    public partial class OpenWeatherMapMeasurement
    {
        public static List<OpenWeatherMapMeasurement> FromJson(string json) => JsonConvert.DeserializeObject<List<OpenWeatherMapMeasurement>>(json, OpenWeatherMapMeasurementConverter.Settings);
    }

    public static class OpenWeatherMapMeasurementSerialize
    {
        public static string ToJson(this List<OpenWeatherMapMeasurement> self) => JsonConvert.SerializeObject(self, OpenWeatherMapMeasurementConverter.Settings);
    }

    public class OpenWeatherMapMeasurementConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
