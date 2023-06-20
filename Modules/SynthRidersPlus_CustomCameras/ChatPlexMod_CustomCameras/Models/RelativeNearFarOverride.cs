using Newtonsoft.Json;
using System;

namespace ChatPlexMod_CustomCameras.Models
{
    [Serializable]
    public class RelativeNearFarOverride
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Enabled = false;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float RelFar = 2.00f;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float RelNear = 0.01f;
    }
}
