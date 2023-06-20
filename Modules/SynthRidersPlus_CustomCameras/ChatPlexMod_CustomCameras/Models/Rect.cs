using Newtonsoft.Json;
using System;

namespace ChatPlexMod_CustomCameras.Models
{
    /// <summary>
    /// Serializable Rect object
    /// </summary>
    [Serializable]
    public class Rect
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _help = "Coordinates are between 0 (bottom left corner) and 1 (top right corner)";
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float PosX = 0f;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float PosY = 0f;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float Width = 0.2f;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float Height = 0.2f;
    }
}