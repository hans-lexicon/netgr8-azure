using AspNet_WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNet_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private List<DeviceItem> _devices = new List<DeviceItem>
        {
            new DeviceItem
            {
                DeviceId = Guid.NewGuid().ToString(),
                Placement = "Livingroom",
                Temperature = 28,
                Humidity = 44,
                AlertNotification = true,
                IsActive = true
            },
            new DeviceItem
            {
                DeviceId = Guid.NewGuid().ToString(),
                Placement = "Bedroom",
                Temperature = 27,
                Humidity = 47,
                AlertNotification = true,
                IsActive = true
            }
        };

        [HttpGet]
        [Route("getdevices")]
        public IActionResult GetDevices()
        {
            return new OkObjectResult(_devices);
        }


    }
}
