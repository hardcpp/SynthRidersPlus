using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChatPlexMod_CustomCameras.Models
{
    /// <summary>
    /// Serializable Config object
    /// </summary>
    [Serializable]
    public class Config
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _help = "Insert the lists below the name of the cameras you want to include like 'SideCam' separated by a comma";
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> MenuCameras = new List<string>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> GameplayCameras = new List<string>();
    }
}