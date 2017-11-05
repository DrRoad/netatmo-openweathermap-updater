using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetatmoOpenWeatherMapUpdater
{
    public class TableStorageMeasurement : TableEntity
    {
        public double Temperature { get; set; }
        public long Humidity { get; set; }
    }
}
