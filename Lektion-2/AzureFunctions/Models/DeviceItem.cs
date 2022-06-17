using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctions.Models
{
    public class DeviceItem
    {
        public string Id { get; set; } = null!;
        public string Placement { get; set; } = null!;
        public string SensorType { get; set; } = null!;
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
        public bool AlertNotification { get; set; }
        public bool IsActive { get; set; }
    }
}
