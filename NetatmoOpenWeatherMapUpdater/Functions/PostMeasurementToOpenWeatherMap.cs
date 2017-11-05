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
    public static class PostMeasurementToOpenWeatherMap
    {
        // Names of the environment variables to get the values from
        private const string EnvOpenWeatherMapAppKey = "OPENWEATHERMAP_KEY";
        private const string EnvOpenWeatherMapStationId = "OPENWEATHERMAP_STATION_ID";

        // URIs
        private const string OpenWeatherMapPostMeasurementUri = "http://api.openweathermap.org/data/3.0/measurements";

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        [FunctionName("UpdateOpenWeatherMap")]
        public static async Task Run(
            [QueueTrigger("%QUEUE_MEASUREMENTS%", Connection = "StorageConnectionString")]NetatmoStationData measurements,
            TraceWriter log)
        {
            log.Info($"Posting measurements to OpenWeatherMap at: {DateTime.Now.ToString(DateTimeFormat)}");

            await PostMeasurement(measurements, log);
        }

        private static async Task<bool> PostMeasurement(NetatmoStationData measurements, TraceWriter log)
        {
            bool success = true;
            using (var http = new HttpClient())
            {
                var key = Environment.GetEnvironmentVariable(EnvOpenWeatherMapAppKey);
                var stationId = Environment.GetEnvironmentVariable(EnvOpenWeatherMapStationId);
                if (string.IsNullOrEmpty(key))
                {
                    log.Error($"ERROR no {EnvOpenWeatherMapAppKey} set");
                    return false;
                }
                if (string.IsNullOrEmpty(stationId))
                {
                    log.Error($"ERROR no {EnvOpenWeatherMapStationId} set");
                    return false;
                }

                var uri = $"{OpenWeatherMapPostMeasurementUri}?APPID={key}";

                var postJSON = CreatePostJSON(measurements, stationId, log);
                if (!string.IsNullOrEmpty(postJSON))
                {

                    log.Info($"Post JSON: {postJSON}");
                    var request = new HttpRequestMessage(HttpMethod.Post, uri) {
                        Content = new StringContent(postJSON, Encoding.UTF8, "application/json")
                    };
                    var response = await http.SendAsync(request);

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
                } else
                {
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
