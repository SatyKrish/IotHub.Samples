using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace IotServiceConsole
{
    internal sealed class MessageReceiver
    {
        EventHubClient _eventHubClient;

        internal MessageReceiver(string iotHubConnectionString)
        {
            _eventHubClient = EventHubClient.CreateFromConnectionString(iotHubConnectionString);                
        }

        internal async Task ReceiveMessagesFromDeviceAsync(CancellationToken cancellationToken)
        {
            var partitions = (await _eventHubClient.GetRuntimeInformationAsync()).PartitionIds;
            var tasks = partitions.Select(async partition =>
            {
                var eventHubReceiver = _eventHubClient.CreateReceiver("$Default", partition, DateTime.UtcNow);
                while (!cancellationToken.IsCancellationRequested)
                {
                    var events = await eventHubReceiver.ReceiveAsync(10);
                    if (!events.Any()) continue;

                    events.ToList().ForEach(eventData =>
                    {
                        var messageString = Encoding.UTF8.GetString(eventData.Body.ToArray());
                        dynamic data = JsonConvert.DeserializeObject(messageString);
                        Console.WriteLine("{0}|{1} > Message received: {2}", DateTime.Now, data.deviceId, messageString);
                    });
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
