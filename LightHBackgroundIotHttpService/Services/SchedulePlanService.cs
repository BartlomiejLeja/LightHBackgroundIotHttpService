using System;
using System.Collections.Generic;
using Windows.System.Threading;
using LightHBackgroundIotHttpService.DTO;
using Newtonsoft.Json;

namespace LightHBackgroundIotHttpService.Services
{
    internal class SchedulePlanService
    {
        //TODO Change for one schedule for all lightsbulb not one service per schedule
        ThreadPoolTimer periodicTimer;
        public void SmartSwitchingLights(MQTTServer mQTTServer, string schedulePlanJson)
        {
            var delay = TimeSpan.FromSeconds(60);
            var timer = 0;
          
             periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(
                async (source) =>
                {
                    timer++;
                    var schedulePlan = JsonConvert.DeserializeObject<List<OneMinuteScheduleDTO>>(schedulePlanJson);
                    foreach (var minute in schedulePlan)
                    {
                        var payLoad = $"light/{minute.ID}/{minute.Status}";
                        await mQTTServer.PublishMessage(payLoad);
                    }
                    if (timer == 4)
                    {
                        periodicTimer.Cancel();
                    }
                }, delay);
        }
    }
}
