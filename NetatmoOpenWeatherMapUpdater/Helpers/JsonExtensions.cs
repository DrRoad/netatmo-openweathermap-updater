using Newtonsoft.Json;
using System.Collections.Generic;

namespace NetatmoOpenWeatherMapUpdater.Helpers
{
    public static class JsonExtensions
    {
        public static string ToJson(this SlackMessage self) => JsonConvert.SerializeObject(self, SlackMessageConverter.Settings);
        public static string ToJson(this NetatmoStationData self) => JsonConvert.SerializeObject(self, NetatmoStationDataConverter.Settings);
        public static string ToJson(this NetatmoAccessToken self) => JsonConvert.SerializeObject(self, NetatmoAccessTokenDataConverter.Settings);
        public static string ToJson(this List<OpenWeatherMapMeasurement> self) => JsonConvert.SerializeObject(self, OpenWeatherMapMeasurementConverter.Settings);
    }
}
