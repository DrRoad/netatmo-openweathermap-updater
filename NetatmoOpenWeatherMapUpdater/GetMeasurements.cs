using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

namespace NetatmoOpenWeatherMapUpdater
{
    public static class GetMeasurements
    {
        // Names of the environment variables to get the values from
        private const string EnvNetatmoClientId = "NETATMO_CLIENT_ID";
        private const string EnvNetatmoClientSecret = "NETATMO_CLIENT_SECRET";
        private const string EnvNetatmoEmail = "NETATMO_EMAIL";
        private const string EnvNetatmoPassword = "NETATMO_PASSWORD";
        private const string EnvNetatmoWeatherStationId = "NETATMO_STATION_ID";
        private const string EnvNetatmoAccessToken = "NETATMO_ACCESS_TOKEN";
        private const string EnvNetatmoAccessTokenExpiresAt = "NETATMO_ACCESS_TOKEN_EXPIRES_AT";
        private const string EnvNetatmoRefreshToken = "NETATMO_REFRESH_TOKEN";
        private const string EnvNetatmoAccessTokenRefreshAsNeeded = "NETATMO_NEED_TO_REFESH_TOKEN";

        // URIs
        private const string NetatmoUriAccessToken = "https://api.netatmo.com/oauth2/token";
        private const string NetatmoUriGetStationData = "https://api.netatmo.com/api/getstationsdata";

        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";


        /// <summary>
        /// Asynchronous function triggered every 20 minutes.
        /// </summary>
        /// <param name="myTimer">Timer trigger</param>
        /// <param name="log">TraceWriter to log to</param>
        /// <returns></returns>
        [FunctionName("GetMeasurements")]
        public static async Task Run(
            [TimerTrigger("0 */20 * * * *")]TimerInfo myTimer,
            [Queue("%QUEUE_MEASUREMENTS%", Connection = "StorageConnectionString")] ICollector<NetatmoStationData> outputQueueItem,
            TraceWriter log)
        {
            log.Info($"Getting measurements from Netatmo weather station at: {DateTime.Now.ToString(DateTimeFormat)}");

            // First get the Netatmo API access token
            var accessToken = await GetAccessToken(log);

            if (string.IsNullOrEmpty(accessToken))
            {
                log.Error($"ERROR could not get or update the Netatmo API access token.");
                return;
            }

            // Then get the Netamo station Id from environment variables
            var netatmoStationId = Environment.GetEnvironmentVariable(EnvNetatmoWeatherStationId);
            if (string.IsNullOrEmpty(netatmoStationId))
            {
                log.Error($"ERROR could not get the Netatmo Station ID from environment variables.");
                return;
            }

            // Then get the measurement from the station via Netatmo API
            var measurements = await GetStationData(accessToken, netatmoStationId, log);
            if (measurements == null)
            {
                log.Error($"ERROR could not get data from the Netatmo API.");
                return;
            }


            var temperature = measurements.Body?.Devices?[0]?.Modules?[0]?.DashboardData?.Temperature;
            var humidity = measurements.Body?.Devices?[0]?.Modules?[0]?.DashboardData?.Humidity;
            //var pressure = measurements.Body?.Devices?[0]?.Modules?[0]?.DashboardData?.Pressure;
            log.Info($"Got measurements: temperature = {temperature}, humidity = {humidity}");

            // For decoupling we just publish the measurement to our Storage Queue and let PostMeasurement handle it
            outputQueueItem.Add(measurements);
        }


        /// <summary>
        /// Get an access token for the Netatmo API.
        /// </summary>
        /// <param name="log">TraceWriter to log to</param>
        /// <returns>string</returns>
        private static async Task<string> GetAccessToken(TraceWriter log)
        {
            string accessToken = string.Empty;

            // If we have a non-expired access token
            if (HaveValidAccessToken(log))
            {
                accessToken = Environment.GetEnvironmentVariable(EnvNetatmoAccessToken);
            }
            else
            {
                NetatmoAccessToken tokenObject = null;

                // Check if we have already received an initial token and need to only refresh
                var needRefresh = Environment.GetEnvironmentVariable(EnvNetatmoAccessTokenRefreshAsNeeded);

                if (string.IsNullOrEmpty(needRefresh) || !needRefresh.Equals("true"))
                {
                    log.Info($"Getting a new access token...");
                    tokenObject = await GetNewAccessToken(log);
                }
                else
                {
                    log.Info($"Refreshing the access token...");
                    tokenObject = await RefreshAccessToken(log);
                }
                if (tokenObject != null)
                {
                    var now = DateTime.UtcNow;
                    var expiresAt = DateTime.UtcNow.AddSeconds(tokenObject.ExpiresIn);

                    Environment.SetEnvironmentVariable(EnvNetatmoAccessTokenRefreshAsNeeded, "true");
                    Environment.SetEnvironmentVariable(EnvNetatmoAccessToken, tokenObject.AccessToken);
                    Environment.SetEnvironmentVariable(EnvNetatmoAccessTokenExpiresAt, expiresAt.ToString());
                    Environment.SetEnvironmentVariable(EnvNetatmoRefreshToken, tokenObject.RefreshToken);
                }

                accessToken = tokenObject?.AccessToken;
            }


            return accessToken;
        }

        /// <summary>
        /// Check from the environment variables that we have an existing access token that is still valid.
        /// </summary>
        /// <param name="log">TraceWriter to log to</param>
        /// <returns>bool</returns>
        private static bool HaveValidAccessToken(TraceWriter log)
        {
            var accessTokenExpiresAtString = Environment.GetEnvironmentVariable(EnvNetatmoAccessTokenExpiresAt);

            if (string.IsNullOrEmpty(accessTokenExpiresAtString))
            {
               return false;
            }

            if (!DateTime.TryParse(accessTokenExpiresAtString, out DateTime accessTokenExpiresAt))
            {
                log.Error($"Could not parse the access token expiry from {accessTokenExpiresAtString}");
                return false;
            }
            var now = DateTime.UtcNow;

            if (now.CompareTo(accessTokenExpiresAt) > 0)
            {
                log.Info($"Access token has expired, need to get new one");
                return false;
            } else
            {
                var diff = accessTokenExpiresAt - now;
                log.Info($"We still have valid access token for {diff} until {accessTokenExpiresAt.ToString()}");
            }

            return true;
        }

        /// <summary>
        /// Refresh the access token for the Netatmo API
        /// </summary>
        /// <param name="log">TraceWriter to log to</param>
        /// <returns>NetatmoAccessToken</returns>
        private static async Task<NetatmoAccessToken> RefreshAccessToken(TraceWriter log)
        {
            NetatmoAccessToken accessToken = null;
            using (var http = new HttpClient())
            {
                var clientId = Environment.GetEnvironmentVariable(EnvNetatmoClientId);
                var parameters = new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "client_secret", Environment.GetEnvironmentVariable(EnvNetatmoClientSecret) },
                    { "username", Environment.GetEnvironmentVariable(EnvNetatmoEmail) },
                    { "password", Environment.GetEnvironmentVariable(EnvNetatmoPassword) },
                    { "grant_type", "refresh_token" },
                    { "scope", "read_station" },
                    { "token_type", "bearer"},
                    { "access_token", Environment.GetEnvironmentVariable(EnvNetatmoAccessToken)},
                    { "refresh_token", Environment.GetEnvironmentVariable(EnvNetatmoRefreshToken)},
                };

                var request = new HttpRequestMessage(HttpMethod.Post, NetatmoUriAccessToken) { Content = new FormUrlEncodedContent(parameters) };
                var response = await http.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    accessToken = NetatmoAccessToken.FromJson(json);
                }
                else
                {
                    log.Error($"ERROR getting the access token: HTTP Status {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            return accessToken;
        }

        /// <summary>
        /// Get a new access token for the Netatmo API
        /// </summary>
        /// <param name="log">TraceWriter to log to</param>
        /// <returns>NetatmoAccessToken</returns>
        private static async Task<NetatmoAccessToken> GetNewAccessToken(TraceWriter log)
        {
            NetatmoAccessToken accessToken = null;
            using (var http = new HttpClient())
            {
                var clientId = Environment.GetEnvironmentVariable(EnvNetatmoClientId);
                var parameters = new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "client_secret", Environment.GetEnvironmentVariable(EnvNetatmoClientSecret) },
                    { "username", Environment.GetEnvironmentVariable(EnvNetatmoEmail) },
                    { "password", Environment.GetEnvironmentVariable(EnvNetatmoPassword) },
                    { "grant_type", "password" },
                    { "scope", "read_station" }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, NetatmoUriAccessToken) { Content = new FormUrlEncodedContent(parameters) };
                var response = await http.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    accessToken = NetatmoAccessToken.FromJson(json);
                }
                else
                {
                    log.Error($"ERROR getting the access token: HTTP Status {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            return accessToken;
        }

        /// <summary>
        /// Get weather station data from the Netatmo API
        /// </summary>
        /// <param name="accessToken">Access token for the API.</param>
        /// <param name="netatmoStationId">Netatmo station ID</param>
        /// <param name="log">TraceWriter to log to.</param>
        /// <returns>NetatmoStationData</returns>
        private static async Task<NetatmoStationData> GetStationData(string accessToken, string netatmoStationId, TraceWriter log)
        {
            NetatmoStationData data = null;
            log.Info($"Getting weather data from Netatmo station {netatmoStationId}");
            using (var http = new HttpClient())
            {
                var uri = $"{NetatmoUriGetStationData}?access_token={accessToken}&device_id={netatmoStationId}";

                var response = await http.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    data = NetatmoStationData.FromJson(json);
                }
                else
                {
                    log.Error($"ERROR getting the access token: HTTP Status {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            return data;
        }
    }
}
