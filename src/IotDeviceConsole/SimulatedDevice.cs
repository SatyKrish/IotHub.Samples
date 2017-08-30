using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace IotDeviceConsole
{
    internal sealed class SimulatedDevice
    {
        readonly DeviceClient _deviceClient;
        readonly Random rand = new Random();
        readonly string _deviceId;

        int _messageId = 1;
        double minTemperature = 20;
        double minHumidity = 60;

        internal SimulatedDevice(string iotHubUri, string deviceId, string deviceKey)
        {
            _deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Mqtt);
            _deviceId = deviceId;
        }

        internal async Task ReceiveCloudToDeviceMessagesAsync(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                var message = await _deviceClient.ReceiveAsync();
                if (message == null) continue;

                var messageString = Encoding.UTF8.GetString(message.GetBytes());
                dynamic data = JsonConvert.DeserializeObject(messageString);
                if (data == null) continue;

                Console.WriteLine("{0}|{1} > Message received: {2}", DateTime.Now, _deviceId, messageString);
                minTemperature = data.minTemperature;
                minHumidity = data.minHumidity;

                await _deviceClient.CompleteAsync(message.LockToken);
            }
        }

        internal async Task SendDeviceToCloudMessagesAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var currentTemperature = minTemperature + rand.NextDouble() * 15;
                var currentHumidity = minHumidity + rand.NextDouble() * 20;

                var telemetryDataPoint = new
                {
                    messageId = _messageId++,
                    deviceId = _deviceId,
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.UTF8.GetBytes(messageString));
                message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                await _deviceClient.SendEventAsync(message);
                Console.WriteLine("{0}|{1} > Sending message: {2}", DateTime.Now, _deviceId, messageString);

                await Task.Delay(5000);
            }
        }
    }
}
