namespace NetatmoOpenWeatherMapUpdater
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class NetatmoAccessToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expire_in")]
        public long ExpireIn { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("scope")]
        public List<string> Scope { get; set; }
    }

    public partial class NetatmoAccessToken
    {
        public static NetatmoAccessToken FromJson(string json) => JsonConvert.DeserializeObject<NetatmoAccessToken>(json, NetatmoAccessTokenDataConverter.Settings);
    }

    public static class SerializeNetatmoAccessToken
    {
        public static string ToJson(this NetatmoAccessToken self) => JsonConvert.SerializeObject(self, NetatmoAccessTokenDataConverter.Settings);
    }

    public class NetatmoAccessTokenDataConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
