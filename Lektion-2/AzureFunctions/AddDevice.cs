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
using AzureFunctions.Models;

namespace AzureFunctions
{
    public static class AddDevice
    {
        private static readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("ServiceClient"));

        [FunctionName("AddDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var deviceItem = JsonConvert.DeserializeObject<DeviceItem>(await new StreamReader(req.Body).ReadToEndAsync());

            var device = await registryManager.AddDeviceAsync(new Device(deviceItem.Id));
            var twin = await registryManager.GetTwinAsync(device.Id);

            twin.Properties.Desired["placement"] = deviceItem.Placement;
            await registryManager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);

            return new OkObjectResult(device.Authentication.SymmetricKey.PrimaryKey);
        }
    }
}
