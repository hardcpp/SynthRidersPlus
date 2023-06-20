using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatPlexMod_CustomCameras.Models
{
    /// <summary>
    /// Serializable Camera config
    /// </summary>
    [Serializable]
    public class Camera
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool IsFirstPerson = false;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool SmoothCamera = false;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Skybox = true;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool NoSkyboxTransparent = false;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float SmoothingFactor = 1.0f;
#if SYNTHRIDERS
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float BloomIntensity = 0.15f;
#endif
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Layer = 10;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float FOV = 80f;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Rect Rect = new Rect();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Position Position = new Position();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Rotation Rotation = new Rotation();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public RelativeNearFarOverride RelativeNearFarOverride = new RelativeNearFarOverride();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, bool> VisibilityLayers = new Dictionary<string, bool>();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [JsonIgnore]
        public int LayerMask = -1;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Validate the config
        /// </summary>
        public void Validate()
        {
            List<string> l_Names = new List<string>();
            for (int l_I = 0; l_I <= 31; l_I++)
            {
                var l_Name = UnityEngine.LayerMask.LayerToName(l_I);
                if (l_Name.Length > 0)
                    l_Names.Add(l_Name);
            }

            var l_NewDic = new Dictionary<string, bool>();
            var l_DefautlOn = new string[]
            {
#if WALKABOUTMINIGOLF
                "Default",
                "TransparentFX",
                "Ignore Raycast",
                "Water",
                "UI",
                "Player",
                "ARPlanes",
                "Ball",
                "PutterCollider",
                "BallNearCup",
                "Cup",
                "PlayerCollisionsOnly",
                "BallCollisionsOnly",
                "CupVisible",
                "CupCutout",
                "Pins",
                "PutterXRayGeo",
                "Avatar",
                "VR_GUI",
                "PutterGeo",
                "AnimatedObjects",
                "LostBalls",
                "Hidden",
                "LODTrigger",
                "Special1",
                "Special2",
                "AvatarEditor"
#elif SYNTHRIDERS
                "Default",
                "TransparentFX",
                "Ignore Raycast",
                "UI",
                "Stage",
                "ScoreUI",
                "MainDisplay",
                "ControllersTrail",
                "StageUI",
                "StatusScoreData",
                "PostProcessing",
                "Notes",
                "Controller Indicator",
                "RoomMenu",
                "WallObstacles"
#elif AUDIOTRIP
                "Default",
                "TransparentFX",
                "Ignore Raycast",
                "Water",
                "UI",
                "Left Hand",
                "Right Hand",
                "Head",
                "Ignore Directional Light",
                "Gameplay Collideable",
                "CustomPostEffectIgnore",
                "HubWorld",
                "PlayerOnly",
                "Left Foot",
                "Right Foot",
                "ExcludeMR"
#else
#error Missing game implementation
#endif
            };

            foreach (var l_Name in l_Names)
            {
                if (VisibilityLayers.ContainsKey(l_Name))
                    l_NewDic.Add(l_Name, VisibilityLayers[l_Name]);
                else
                    l_NewDic.Add(l_Name, l_DefautlOn.Contains(l_Name));
            }

            VisibilityLayers    = l_NewDic;
            LayerMask           = UnityEngine.LayerMask.GetMask(l_NewDic.Where(x => x.Value == true).Select(x => x.Key).ToArray());
        }
    }
}