using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using NetatmoOpenWeatherMapUpdater.Helpers;
using System;

namespace NetatmoOpenWeatherMapUpdater
{
    public static class StoreToTableStorage
    {
        [FunctionName("StoreToTableStorage")]
        public static void Run(
            [QueueTrigger("%QUEUE_TABLESTORAGE%", Connection = "StorageConnectionString")]NetatmoStationData measurements,
            [Table("%TABLE_MEASUREMENTS%", Connection = "StorageConnectionString")] ICollector<TableStorageMeasurement> tableStorage,
            TraceWriter log)
        {
            var temperature = measurements.Body?.Devices?[0]?.Modules?[0]?.DashboardData?.Temperature;
            var humidity = measurements.Body?.Devices?[0]?.Modules?[0]?.DashboardData?.Humidity;
            var timestampOption = measurements.Body?.Devices?[0]?.Modules?[0]?.DashboardData?.TimeUtc;
            if (!timestampOption.HasValue)
            {
                log.Error("ERROR no timestamp.");
                return;
            }

            DateTime timestamp = timestampOption.Value.FromUnixTimestampToDateTime();

            var tableEntity = new TableStorageMeasurement()
            {
                PartitionKey = measurements.Body?.Devices?[0]?.Modules?[0]?.Id,
                RowKey = $"{timestampOption.Value}",
                Temperature = temperature.Value,
                Humidity = humidity.Value
            };

            tableStorage.Add(tableEntity);
        }
    }
}
