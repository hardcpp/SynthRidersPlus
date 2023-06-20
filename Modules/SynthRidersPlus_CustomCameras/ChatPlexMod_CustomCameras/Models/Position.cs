using Newtonsoft.Json;
using System;

namespace ChatPlexMod_CustomCameras.Models
{
    /// <summary>
    /// Serializable Position object
    /// </summary>
    [Serializable]
    public class Position
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _help = "Position of the camera in the scene relative to current room center.";
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float X = 0f;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float Y = 1f;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float Z = 0f;
    }
}