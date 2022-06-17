using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotDevice_WPF.Models
{
    public class DeviceItem
    {
        public string Id { get; set; } = null!;
        public string SensorType { get; set; } = null!;
        public string SharedAccessKey { get; set; } = null!;
    }
}
