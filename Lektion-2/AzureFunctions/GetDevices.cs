using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunctions.Models;
using System.Collections.Generic;
using Microsoft.Azure.Devices;

namespace AzureFunctions
{
    public static class GetDevices
    {
        private static readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("ServiceClient"));


        [FunctionName("GetDevices")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "devices")] HttpRequest req,
            ILogger log)
        {

            var devices = new List<DeviceItem>();

            var result = registryManager.CreateQuery("SELECT * FROM devices");
            if(result.HasMoreResults)
                foreach(var twin in await result.GetNextAsTwinAsync())
                {
                    var device = new DeviceItem()
                    {
                        Id = twin.DeviceId,
                        IsActive = (twin.ConnectionState.ToString() == "Connected") ? true : false,
                    };

                    try { device.Placement = twin.Properties.Desired["placement"]; }
                    catch { device.Placement = ""; }

                    try { device.SensorType = twin.Properties.Reported["sensorType"]; }
                    catch { device.SensorType = ""; }

                    try { device.AlertNotification = twin.Properties.Reported["alertNotification"]; }
                    catch { device.AlertNotification = false; }

                    try { device.Temperature = twin.Properties.Reported["temperature"]; }
                    catch { }

                    try { device.Humidity = twin.Properties.Reported["humidity"]; }
                    catch { }

                    devices.Add(device);
                }

            return new OkObjectResult(devices);
        }
    }
}
