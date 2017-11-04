using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace NetatmoOpenWeatherMapUpdater
{
    public static class GetMeasurements
    {
        [FunctionName("GetMeasurements")]
        public static void Run([TimerTrigger("0 */20 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"Getting measurements from weather station at: {DateTime.Now}");
        }
    }
}
