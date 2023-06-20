using MelonLoader;

namespace SynthRidersPlus_ChatEmoteRain
{
    /// <summary>
    /// Melon main class
    /// </summary>
    public class Mod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            ChatPlexMod_ChatEmoteRain.Logger.Instance = new CP_SDK.Logging.MelonLoaderLogger();
        }
    }
}
