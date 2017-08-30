using System;
using System.Threading.Tasks;
using System.Threading;

namespace IotDeviceConsole
{
    class Program
    {
        const string IotHubUri = "SatyIotSample1.azure-devices.net";
        const string DeviceId = "Device001";
        const string DeviceKey = "SQx9YJb2cccEPZTYz1dGo4kmC11j4MUtxnHtl2nGLwE=";

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated Device !\n");
            var simulatedDevice = new SimulatedDevice(IotHubUri, DeviceId, DeviceKey);

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var receiveTask = simulatedDevice.ReceiveCloudToDeviceMessagesAsync(cts.Token);
            var sendTask = simulatedDevice.SendDeviceToCloudMessagesAsync(cts.Token);
            Task.WaitAll(receiveTask, sendTask);
        }
    }
}