using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LightHBackgroundIotHttpService.Services
{
    internal class WiFiService 
    {
        private HttpListener _listener;

        public string IsOnLightStatus { get; set; }

        public async Task SendHttpRequestAsync(bool lightStatus,int lightNumber, LightsService lightsService, SignalRService signalRService)
        {
            var httpClient = new HttpClient();
            var httpResponseBody = string.Empty;
            Uri requestUri;

            if (lightStatus)
            {
                requestUri = new Uri($"http://192.168.1.{lightNumber}/control?cmd=GPIO,0,0");
                IsOnLightStatus = "On";
            }
            else
            {
                requestUri = new Uri($"http://192.168.1.{lightNumber}/control?cmd=GPIO,0,1");
                IsOnLightStatus = "Off";
            }

            var httpResponse = new HttpResponseMessage();
            Debug.WriteLine("TestService");
            try
            {
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
            Debug.WriteLine(httpResponseBody);
            lightsService.LightModelList.First(light => light.ID == lightNumber).LightStatus = lightStatus;

            await signalRService.InvokeSendStatusMethod(lightNumber, lightStatus);
        }

        public async Task<string> CheckStatusOfLight(int lightID)
        {
            Debug.WriteLine("Check Status Test");
            var httpClient = new HttpClient();
            var requestUri = new Uri($"http://192.168.1.{lightID}/control?cmd=STATUS,GPIO,0");

            var httpResponse = new HttpResponseMessage();
            
            using (httpResponse = await httpClient.GetAsync(requestUri).ConfigureAwait(false))
            {
                return await httpResponse.Content.ReadAsStringAsync();
            }
        }

        public async Task ListenHttpRequestsAsync(SignalRService signalRService, LightsService lightsService)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://+:8081/");
            _listener.Start();
            Debug.WriteLine("Listning started");
            Debug.WriteLine(_listener.GetContextAsync());
          
            while (true)
            {
                var context = await _listener.GetContextAsync();
                var response = context.Response;
                const string responseString = "<html><body>Hello world</body></html>";

                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                response.ContentLength64 = buffer.Length;

                var output = response.OutputStream;

                output.Write(buffer, 0, buffer.Length);

                var urlRequestTab = (context.Request.Url).ToString().Split("=");
                var urlNumber = urlRequestTab[3].Substring(0, 3);

                IsOnLightStatus = urlRequestTab[urlRequestTab.Length - 1];
                var isOn = IsOnLightStatus == "On";

                if (lightsService.LightModelList.Any(light => light.ID == int.Parse(urlNumber)))
                {
                    lightsService.LightModelList.First(light => light.ID == int.Parse(urlNumber)).LightStatus = isOn;

                    await signalRService.InvokeSendStatusMethod(int.Parse(urlNumber), isOn);
                }
                
                Debug.WriteLine(IsOnLightStatus);
            }
        }
    }
}
