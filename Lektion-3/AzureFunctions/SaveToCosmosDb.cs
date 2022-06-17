using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using ClassLibrary.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions
{
    public static class SaveToCosmosDb
    {
        [FunctionName("SaveToCosmosDb")]
        public static void Run(
            [EventHubTrigger("netgr8-iothub", Connection = "IotHubEndpoint", ConsumerGroup = "cosmosdb")] EventData events,
            [CosmosDB(databaseName: "NETGR8", collectionName: "Messages", CreateIfNotExists = true, ConnectionStringSetting = "CosmosDB")] out dynamic cosmos,
            ILogger log)
        {
            try
            {
                var measurementModel = JsonConvert.DeserializeObject<MeasurementModel>(events.EventBody.ToString());

                measurementModel.Id = Guid.NewGuid();
                measurementModel.DeviceId = events.SystemProperties["iothub-connection-device-id"].ToString();
                measurementModel.MeasurementTime = DateTime.Now;
                measurementModel.AlertNotification = (events.Properties["alertNotification"].ToString() == "true");

                cosmos = measurementModel;
            }
            catch
            {
                cosmos = null;
            }
        }
    }
}
