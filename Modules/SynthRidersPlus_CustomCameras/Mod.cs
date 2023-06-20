using MelonLoader;

namespace SynthRidersPlus_CustomCameras
{
    /// <summary>
    /// Melon main class
    /// </summary>
    public class Mod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            ChatPlexMod_CustomCameras.Logger.Instance = new CP_SDK.Logging.MelonLoaderLogger();
        }
    }
}
