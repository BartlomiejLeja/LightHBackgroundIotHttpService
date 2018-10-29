using MQTTnet;
using MQTTnet.Server;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace LightHBackgroundIotHttpService.Services
{
    internal class MQTTServer
    {
        private IMqttServer mqttServer;
        public async Task ServerRun(SignalRService signalRService)
        {
            Debug.WriteLine("TestMQTTServer");
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(100)
                .WithDefaultEndpointPort(8081);

            mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.ApplicationMessageReceived += async (s, e) =>
            {
                Debug.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Debug.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Debug.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Debug.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Debug.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Debug.WriteLine($"+ Retain = {e.ClientId}");
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var splitedPayload = payload.Split('/');
                var isOn = splitedPayload[2] == "on" ? true : false;
                await signalRService.InvokeSendStatusMethod(int.Parse(splitedPayload[1]), isOn);
            };
            await mqttServer.StartAsync(optionsBuilder.Build());

//            Console.WriteLine("Press any key to exit.");
//            Console.ReadLine();
//            await mqttServer.StopAsync();
        }

        public async Task PublishMessage(string payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("LightBulb")
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await mqttServer.PublishAsync(message);
        }
    }
}
