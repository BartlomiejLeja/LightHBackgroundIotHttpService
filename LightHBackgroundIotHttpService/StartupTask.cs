using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Background;
using LightHBackgroundIotHttpService.Services;
using static Windows.ApplicationModel.Background.BackgroundAccessStatus;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace LightHBackgroundIotHttpService
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static BackgroundTaskDeferral _Deferral = null;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _Deferral = taskInstance.GetDeferral();

            var signalServer = new SignalRService();
         
            var lightService = new LightsService();
          //  var chartService = new ChartService(lightService);
            var schedulePlan = new SchedulePlanService();
           
            var mqttService = new MQTTServer();
            await signalServer.Connect(lightService, mqttService, schedulePlan);
         
            await mqttService.ServerRun(signalServer);

            //            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            //            var taskName = "test";
            //            if (backgroundAccessStatus == AlwaysAllowed)
            //            {
            //                foreach (var task in BackgroundTaskRegistration.AllTasks)
            //                {
            //                    if (task.Value.Name == taskName)
            //                    {
            //                        return;
            //                    }
            //                }
            //            }
            //
            //            BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
            //            taskBuilder.Name = taskName;
            //            taskBuilder.TaskEntryPoint = 
//            var status = await BackgroundExecutionManager.RequestAccessAsync();
//            var taskName = "test";
//            if (BackgroundTaskRegistration.AllTasks.All(x => x.Value.Name != taskName))
//            {
//                BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
//                builder.Name = taskName;
//                builder.TaskEntryPoint = typeof(LightHBackgroundIotHttpService.TileUpdateTask).FullName;
//                builder.SetTrigger(new TimeTrigger(16,false));
//               
//                if (status != BackgroundAccessStatus.Denied)
//                {
//                    builder.Register();
//                }
//            }

            // chartService.SmartSwitchingLights(true, lightService);

            //            await ThreadPool.RunAsync(workItem =>
            //            {
            //                webserver.Start();
            //                
            //            });
        }
    }

//    public sealed class TileUpdateTask : IBackgroundTask
//    {
//        public void Run(IBackgroundTaskInstance taskInstance)
//        {
//            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
//            Debug.WriteLine("BackGroundTaskWorksFine");
//            deferral.Complete();
//        }
//    }
}
