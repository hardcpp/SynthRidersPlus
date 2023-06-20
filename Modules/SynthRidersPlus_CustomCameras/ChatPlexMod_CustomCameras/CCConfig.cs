using Newtonsoft.Json;

namespace ChatPlexMod_CustomCameras
{
    internal class CCConfig : CP_SDK.Config.JsonConfig<CCConfig>
    {
        [JsonProperty] internal bool Enabled = true;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get relative config path
        /// </summary>
        /// <returns></returns>
        public override string GetRelativePath()
            => $"{CP_SDK.ChatPlexSDK.ProductName}/CustomCameras/Config";
    }
}
