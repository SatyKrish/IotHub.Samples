using System;
using System.Threading;
using System.Threading.Tasks;

namespace IotServiceConsole
{
    class Program
    { 
        const string IotHubServiceConnectionString = "HostName=SatyIotSample1.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=5aTHHiIV6XXtKoBz6bR85Jbh3rkwy+r2+XGNXUPD83E=";
        const string IotHubEventHubCompatibleConnectionString = "Endpoint=sb://ihsuprodblres067dednamespace.servicebus.windows.net/;EntityPath=iothub-ehub-satyiotsam-174752-64d1e04f3d;SharedAccessKeyName=iothubowner;SharedAccessKey=cztlbCAJzbdZqJf1xTEn/2+y6x2BW362SpHaGp0Du5g=";
        const string DeviceId = "Device001";

        static void Main(string[] args)
        {
            Console.WriteLine("Iot Service !\n");
            var sender = new MessageSender(IotHubServiceConnectionString);
            var receiver = new MessageReceiver(IotHubEventHubCompatibleConnectionString);

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var sendTask = sender.SendMessagesToDeviceAsync(DeviceId, cts.Token);
            var receiveTask = receiver.ReceiveMessagesFromDeviceAsync(cts.Token);
            Task.WaitAll(sendTask, receiveTask);
        }
    }
}