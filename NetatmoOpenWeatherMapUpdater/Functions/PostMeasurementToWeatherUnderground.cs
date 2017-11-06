using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using NetatmoOpenWeatherMapUpdater.Helpers;

namespace NetatmoOpenWeatherMapUpdater
{
    public static class PostMeasurementToWeatherUnderground
    {
        // Names of the environment variables to get the values from
        private const string EnvWundergroundStationId = "WUNDERGROUND_ID";
        private const string EnvWundergroundStationKey = "WUNDERGROUND_KEY";

        // URIs
        private const string WeatherUndergroundPostMeasurementUri = "https://weatherstation.wunderground.com/weatherstation/updateweatherstation.php";

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        [FunctionName("UpdateWeatherUnderground")]
        public static async Task Run(
            [QueueTrigger("%QUEUE_MEASUREMENTS_WU%", Connection = "StorageConnectionString")]NetatmoStationData measurements,
            TraceWriter log)
        {
            log.Info($"Posting measurements to Weather Underground at: {DateTime.Now.ToString(DateTimeFormat)}");

            await PostMeasurement(measurements, log);
        }

        private static async Task<bool> PostMeasurement(NetatmoStationData measurements, TraceWriter log)
        {
            bool success = true;
            using (var http = new HttpClient())
            {
                var stationId = Environment.GetEnvironmentVariable(EnvWundergroundStationId);
                var key = Environment.GetEnvironmentVariable(EnvWundergroundStationKey);
                if (string.IsNullOrEmpty(key))
                {
                    log.Error($"ERROR no {EnvWundergroundStationKey} set");
                    return false;
                }
                if (string.IsNullOrEmpty(stationId))
                {
                    log.Error($"ERROR no {EnvWundergroundStationId} set");
                    return false;
                }

                var timestamp = measurements.Body?.Devices?[0]?.Modules?[0]?.DashboardData?.TimeUtc;
                var temperature = measurements.Body?.Devices?[0]?.Modules?[0]?.DashboardData?.Temperature;
                var humidity = measurements.Body?.Devices?[0]?.Modules?[0]?.DashboardData?.Humidity;
                var pressure = measurements.Body?.Devices?[0]?.DashboardData?.AbsolutePressure;

                var tempF = UnitConversions.FromCelsiusToFahrenheit(temperature.Value);
                var inHg = UnitConversions.InHgFromMillibars(pressure.Value);
                var uri = $"{WeatherUndergroundPostMeasurementUri}?action=updateraw&dateutc=={timestamp.Value}&tempf={tempF}&humidity={humidity.Value}&baromin={inHg}&ID={stationId}&PASSWORD={key}";

                log.Info($"URI: {uri}");
                var response = await http.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    log.Info($"Measurement posted succesfully. {response.StatusCode} - {response.ReasonPhrase}");
                    success = true;
                }
                else
                {
                    log.Error($"ERROR posting measurement: {response.StatusCode} - {response.ReasonPhrase}");
                    success = false;
                }
            }

            return success;
        }

        /// <summary>
        /// Create the JSON-object to post to the API.
        /// </summary>
        /// <param name="netatmoData">Measurements from Netatmo</param>
        /// <param name="owmStationId">OpenWeatherMap station id to post to</param>
        /// <returns>string</returns>
        private static string CreatePostJSON(NetatmoStationData netatmoData, string owmStationId, TraceWriter log)
        {
            DateTime now = DateTime.UtcNow;
            long unixTime = ((DateTimeOffset)now).ToUnixTimeSeconds();

            var measurements = new List<OpenWeatherMapMeasurement>() {
                new OpenWeatherMapMeasurement()
                {
                    StationId = owmStationId,
                }
            };

            var timestamp = netatmoData.Body?.Devices?[0]?.Modules?[0]?.DashboardData?.TimeUtc;
            var temperature = netatmoData.Body?.Devices?[0]?.Modules?[0]?.DashboardData?.Temperature;
            var humidity = netatmoData.Body?.Devices?[0]?.Modules?[0]?.DashboardData?.Humidity;

            if (timestamp.HasValue)
            {
                measurements[0].Dt = timestamp.Value;
            }
            else
            {
                log.Error($"No timestamp on measurements, cannot send");
                return string.Empty;
            }
            if (temperature.HasValue)
            {
                measurements[0].Temperature = temperature.Value;
            }
            if (humidity.HasValue)
            {
                measurements[0].Humidity = humidity.Value;
            }

            return JsonExtensions.ToJson(measurements);
        }
    }
}
