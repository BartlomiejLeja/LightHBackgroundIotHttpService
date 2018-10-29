using System.Collections.Generic;
using System.Linq;
using LightHBackgroundIotHttpService.Model;

namespace LightHBackgroundIotHttpService.Services
{
    internal class LightsService
    { 
        public List<LightModel> LightModelList = new List<LightModel>();

        public LightModel GetLightModel(int lightBulbID)
        {
            return LightModelList.First(lightBulb => lightBulb.ID == lightBulbID);
        }
    }
}
