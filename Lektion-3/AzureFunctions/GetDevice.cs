using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using ClassLibrary.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace AzureFunctions
{
    public static class GetDevice
    {
        private static readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub_SharedAccessKey"));


        [FunctionName("GetDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "device")] HttpRequest req,
            [CosmosDB(databaseName: "NETGR8", collectionName: "Messages", SqlQuery = "SELECT top 1 * FROM c order by c._ts desc", CreateIfNotExists = true, ConnectionStringSetting = "CosmosDB")] IEnumerable<MeasurementModel> cosmos,
            ILogger log)
        {
            var device = await registryManager.GetDeviceAsync(req.Query["deviceId"]);
            if (device == null)
                return new NotFoundObjectResult("No device found");


            var twin = await registryManager.GetTwinAsync(device.Id);

            var deviceModel = new DeviceModel
            {
                Id = device.Id,
                Online = (device.ConnectionState.ToString() == "Connected"),
                LastActivityTime = twin.LastActivityTime.ToString()
            };

            try { deviceModel.Placement = twin.Properties.Desired["placement"]; }
            catch { deviceModel.Placement = ""; }

            try
            {
                try { deviceModel.SensorType = twin.Properties.Reported["sensorType"]; }
                catch
                {
                    try { deviceModel.SensorType = twin.Properties.Desired["sensorType"]; }
                    catch { deviceModel.SensorType = ""; }
                }
            }
            catch { deviceModel.SensorType = ""; }

            try
            {
                //using (IDbConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnection")))
                //{
                //    var latestMeasurement = await conn.QueryFirstOrDefaultAsync<MeasurementModel>("SELECT * FROM Measurements WHERE DeviceId = @DeviceId ORDER BY MeasurementTime DESC", new { DeviceId = device.Id });
                    var latestMeasurement = cosmos.FirstOrDefault(x => x.DeviceId == device.Id);

                    if (latestMeasurement != null)
                    {
                        try
                        {
                            deviceModel.Temperature = latestMeasurement.Temperature;
                            deviceModel.Humidity = latestMeasurement.Humidity;
                            deviceModel.AlertNotification = latestMeasurement.AlertNotification;
                        }
                        catch { }
                    }
                //}
            }
            catch
            {
                try { deviceModel.Temperature = twin.Properties.Reported["temperature"]; }
                catch { deviceModel.Temperature = 0; }

                try { deviceModel.Humidity = twin.Properties.Reported["humidity"]; }
                catch { deviceModel.Humidity = 0; }

                try { deviceModel.AlertNotification = twin.Properties.Reported["alertNotification"]; }
                catch { deviceModel.AlertNotification = false; }
            }

            return new OkObjectResult(deviceModel);
        }
    }
}
