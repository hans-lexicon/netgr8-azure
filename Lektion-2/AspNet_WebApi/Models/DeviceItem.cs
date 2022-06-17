namespace AspNet_WebApi.Models
{
    public class DeviceItem
    {
        public string DeviceId { get; set; } = null!;
        public string Placement { get; set; } = null!;
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
        public bool AlertNotification { get; set; }
        public bool IsActive { get; set; }

    }
}
