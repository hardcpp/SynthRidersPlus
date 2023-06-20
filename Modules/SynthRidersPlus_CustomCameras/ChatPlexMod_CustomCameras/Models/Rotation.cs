using Newtonsoft.Json;
using System;

namespace ChatPlexMod_CustomCameras.Models
{
    /// <summary>
    /// Serializable Rotation object
    /// </summary>
    [Serializable]
    public class Rotation
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _help = "Rotation of the camera in the scene is measured in Euler angles.";
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float X = 0f;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float Y = 0f;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float Z = 0f;
    }
}