namespace WebApp.Models
{
    public class DeviceItemModel
    {
        public string Id { get; set; } = null!;
        public string Placement { get; set; } = null!;
        public decimal Temperature { get; set; }
        public decimal Humidity { get; set; }
        public bool AlertNotification { get; set; }
        public bool IsActive { get; set; }
    }
}
