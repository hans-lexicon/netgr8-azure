using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using ClassLibrary.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dapper;

namespace AzureFunctions
{
    public static class SaveDataToSql
    {
        [FunctionName("SaveDataToSql")]
        public static async Task Run([EventHubTrigger("netgr8-iothub", Connection = "IotHubEndPoint", ConsumerGroup = "readmessages")] EventData events, ILogger log)
        {
            var measurementModel = JsonConvert.DeserializeObject<MeasurementModel>(events.EventBody.ToString());

            measurementModel.Id = Guid.NewGuid();
            measurementModel.DeviceId = events.SystemProperties["iothub-connection-device-id"].ToString();
            measurementModel.MeasurementTime = DateTime.Now;
            measurementModel.AlertNotification = (events.Properties["alertNotification"].ToString() == "true");

            using (IDbConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnection")))
            {
                await conn.ExecuteAsync("INSERT INTO Measurements (Id,DeviceId,MeasurementTime,Temperature,Humidity,AlertNotification) VALUES(@Id, @DeviceId, @MeasurementTime, @Temperature, @Humidity, @AlertNotification)", measurementModel);
            }
        }
    }
}
