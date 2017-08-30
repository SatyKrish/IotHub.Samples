using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;

namespace IotServiceConsole
{
    internal sealed class MessageSender
    {
        const double MinTemperature = 20;
        const double MinHumidity = 60;

        ServiceClient _serviceClient;

        internal MessageSender(string iotHubConnectionString)
        {
            _serviceClient = ServiceClient.CreateFromConnectionString(iotHubConnectionString);
        }

        public async Task SendMessagesToDeviceAsync(string deviceId, CancellationToken cancellationToken)
        {
            int i = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                var data = new
                {
                    minTemperature = MinTemperature + i,
                    minHumidity = MinHumidity + i
                };
                var messageString = JsonConvert.SerializeObject(data);
                var message = new Message(Encoding.UTF8.GetBytes(messageString));

                await _serviceClient.SendAsync(deviceId, message);
                Console.WriteLine("{0}|{1} > Sending message: {2}", DateTime.Now, deviceId, messageString);
                await Task.Delay(5000);
                i++;
            }
        }
    }
}
