using MelonLoader;

namespace SynthRidersPlus_ChatIntegrations
{
    /// <summary>
    /// Melon main class
    /// </summary>
    public class Mod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            ChatPlexMod_ChatIntegrations.Logger.Instance = new CP_SDK.Logging.MelonLoaderLogger();
        }
    }
}
