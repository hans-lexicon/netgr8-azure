using IotDevice_WPF.Models;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dapper;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace IotDevice_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DeviceItem deviceItem;
        private Random rnd;
        private DeviceClient deviceClient;

        private string hostName = "netgr8-iothub.azure-devices.net";
        private string apiUrl = "https://netgr8-functionapp.azurewebsites.net/api/AddDevice?code=nB9Z0r-Z839sZhjPDOy2MY9NTU3_umfnXnY65QHP-cyCAzFuqYTKgg==";
        private string sql = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\HansMattin-Lassei\source\repos\device_db.mdf;Integrated Security=True;Connect Timeout=30";
        private bool isConfigured = false;


        public MainWindow()
        {
            InitializeComponent();
            InitDevice();

            if (!isConfigured)
            {
                btnConnect.IsEnabled = true;
                btnConnect.Content = "Connect";
            }
            else
            {
                ConnectAndSendAsync().GetAwaiter();
            }
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            using (IDbConnection conn = new SqlConnection(sql))
            {
                deviceItem.Id = Guid.NewGuid().ToString();
                deviceItem.SensorType = "GUI APP";

                using (var http = new HttpClient())
                {
                    var result = await http.PostAsJsonAsync(apiUrl, deviceItem);
                    deviceItem.SharedAccessKey = await result.Content.ReadAsStringAsync();
                }

                conn.Open();
                await conn.ExecuteAsync("INSERT INTO Device VALUES (@Id, @SensorType, @SharedAccessKey)", deviceItem);

            }

            await ConnectAndSendAsync();
        }

        private void InitDevice()
        {
            rnd = new();
            deviceItem = new();

            using(IDbConnection conn = new SqlConnection(sql))
            {
                conn.Open();
                var data = conn.Query<DeviceItem>("SELECT * FROM Device");

                if(data.Any())
                {
                    foreach(var device in data)
                    {
                        deviceItem.Id = device.Id;
                        deviceItem.SensorType = device.SensorType;
                        deviceItem.SharedAccessKey = device.SharedAccessKey;
                    }

                    isConfigured = true;
                }

            }
        }

        private async Task ConnectAndSendAsync()
        {
            tblockDeviceId.Text = deviceItem.Id;
            btnConnect.IsEnabled = false;
            btnConnect.Content = "Connected";

            deviceClient = DeviceClient.CreateFromConnectionString($"HostName={hostName};DeviceId={deviceItem.Id};SharedAccessKey={deviceItem.SharedAccessKey}");
            await UpdateSensorTypeAsync(deviceItem.SensorType);
            await SendMessageAsync();
        }

        private async Task UpdateSensorTypeAsync(string sensorType)
        {
            TwinCollection reportedProperty = new TwinCollection();
            reportedProperty["sensorType"] = sensorType;

            await deviceClient.UpdateReportedPropertiesAsync(reportedProperty);
        }

        private async Task SendMessageAsync()
        {
            while (true)
            {
                var message = new DeviceMessage
                {
                    Temperature = rnd.Next(20, 30),
                    Humidity = rnd.Next(30, 40)
                };

                TwinCollection reportedProperty = new TwinCollection();
                reportedProperty["temperature"] = message.Temperature;
                reportedProperty["humidity"] = message.Humidity;

                await deviceClient.UpdateReportedPropertiesAsync(reportedProperty);
                await deviceClient.SendEventAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))));
                
                tblockInfo.Text = $"Message sent {DateTime.Now}";
                await Task.Delay(30 * 1000);
            }
        }
    }
}
