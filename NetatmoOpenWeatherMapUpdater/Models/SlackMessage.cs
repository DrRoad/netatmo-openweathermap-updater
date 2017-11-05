// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using NetatmoOpenWeatherMapUpdater;
//
//    var data = SlackMessage.FromJson(jsonString);
//
namespace NetatmoOpenWeatherMapUpdater
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class SlackMessage
    {
        [JsonProperty("humidity")]
        public long Humidity { get; set; }

        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
    }

    public partial class SlackMessage
    {
        public static SlackMessage FromJson(string json) => JsonConvert.DeserializeObject<SlackMessage>(json, SlackMessageConverter.Settings);
    }

    public class SlackMessageConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
