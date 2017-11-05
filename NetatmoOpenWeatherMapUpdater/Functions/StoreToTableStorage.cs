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
            [QueueTrigger("%QUEUE_TABLESTORAGE%", Connection = "StorageConnectionString")]NetatmoStationData stationData,
            [Table("%TABLE_MEASUREMENTS%", Connection = "StorageConnectionString")] ICollector<TableStorageMeasurement> tableStorage,
            TraceWriter log)
        {

            foreach (var device in stationData.Body?.Devices)
            {
                log.Info($"Device {device.Id}");

                foreach (var module in device.Modules)
                {
                    log.Info($"Module {module.Id}");
                    var data = module.DashboardData;

                    var moduleDataToStore = CreateModuleDto(device, module, data);
                    tableStorage.Add(moduleDataToStore);
                }

                var deviceDataToStore = CreateDeviceDto(device);
                tableStorage.Add(deviceDataToStore);
            }
        }

        private static TableStorageMeasurement CreateDeviceDto(Device device)
        {
            var data = device.DashboardData;
            return new TableStorageMeasurement()
            {
                PartitionKey = device.Id,
                RowKey = $"{data.TimeUtc}",
                StatioName = device.StationName,
                ModuleName = device.ModuleName,
                ModuleType = device.Type,
                UnixTimestamp = data.TimeUtc,
                Temperature = data.Temperature,
                TemperatureTrend = data.TempTrend,
                MinTemperature = data.MinTemp,
                MaxTemperature = data.MaxTemp,
                MinTemperatureUnixTimestamp = data.DateMinTemp,
                MaxTemperatureUnixTimestamp = data.DateMaxTemp,
                Humidity = data.Humidity,
                CO2 = data.CO2,
                AbsolutePressure = data.AbsolutePressure,
                Pressure = data.Pressure,
                PressureTrend = data.PressureTrend,
                Noise = data.Noise,
                LastMessage = device.LastStatusStore,
                LastSeen = device.LastStatusStore,
                LastSetup = device.LastSetup,
                LastUpgrade = device.LastUpgrade,
                WifiStatus = device.WifiStatus,
                Firmware = device.Firmware,
                Longitude = device.Place?.Location?[1],
                Latitude = device.Place?.Location?[0],
                Altitude = device.Place?.Altitude,
                City = device.Place?.City,
                Country = device.Place?.Country,
                Timezone = device.Place?.Timezone
            };
        }

        private static TableStorageMeasurement CreateModuleDto(Device device, Module module, FluffyDashboardData data)
        {
            return new TableStorageMeasurement()
            {
                PartitionKey = module.Id,
                RowKey = $"{data.TimeUtc}",
                StatioName = device.StationName,
                ModuleName = module.ModuleName,
                ModuleType = module.Type,
                UnixTimestamp = data.TimeUtc,
                Temperature = data.Temperature,
                TemperatureTrend = data.TempTrend,
                MinTemperature = data.MinTemp,
                MaxTemperature = data.MaxTemp,
                MinTemperatureUnixTimestamp = data.DateMinTemp,
                MaxTemperatureUnixTimestamp = data.DateMaxTemp,
                Humidity = data.Humidity,
                CO2 = data.CO2,
                LastMessage = module.LastMessage,
                LastSeen = module.LastSeen,
                LastSetup = module.LastSetup,
                BatteryPercentage = module.BatteryPercent,
                BatteryVp = module.BatteryVp,
                RfStatus = module.RfStatus,
                Firmware = module.Firmware,
                Longitude = device.Place?.Location?[1],
                Latitude = device.Place?.Location?[0],
                Altitude = device.Place?.Altitude,
                City = device.Place?.City,
                Country = device.Place?.Country,
                Timezone = device.Place?.Timezone
            };
        }
    }
}
