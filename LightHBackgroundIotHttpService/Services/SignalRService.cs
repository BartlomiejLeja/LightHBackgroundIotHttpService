using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace LightHBackgroundIotHttpService.Services
{
    internal class SignalRService 
    {
        private HubConnection _connection;
        
        public async Task Connect(LightsService lightsService, MQTTServer mQTTServer,SchedulePlanService schedulePlanService)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://signalirserver20181021093049.azurewebsites.net/LightApp")
                .WithConsoleLogger(LogLevel.Trace)
                .Build();

            _connection.On<bool, int>("ChangeLightState", async (lightStatus,lightNumber) =>
            {
               Debug.WriteLine("You turn on light");
           
               var statusOffLight = lightStatus ? "on" : "off";
               var payLoad = $"light/{lightNumber}/{statusOffLight}";
               await mQTTServer.PublishMessage(payLoad);
            });
            
            _connection.On<string>("SendInitialLightCollection", lightsCollection =>
            {
                Debug.WriteLine(lightsCollection);
          
            });

            _connection.On<string>("SendSchedulePlan", schedulePlanJson =>
            {
                Debug.WriteLine("SendSchedulePlan");
                if (false)
                {
                    schedulePlanService.SmartSwitchingLights(mQTTServer, schedulePlanJson);
                }

            });
            await _connection.StartAsync();
        }
        
        public async Task InvokeSendStatusMethod(int lightID, bool lightStatus)
        {
            Debug.WriteLine("SendLightState");
            await _connection.InvokeAsync("SendLightState", lightID, lightStatus,DateTime.Now,"MOCK");
        }
    }
}
