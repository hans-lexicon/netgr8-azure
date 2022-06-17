using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class MeasurementModel
    {
        public Guid Id { get; set; }
        public string? DeviceId { get; set; }
        public DateTime MeasurementTime { get; set; }
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
        public bool AlertNotification { get; set; }
    }
}
