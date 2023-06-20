using MelonLoader;

namespace SynthRidersPlus_Chat
{
    /// <summary>
    /// Melon main class
    /// </summary>
    public class Mod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            ChatPlexMod_Chat.Logger.Instance = new CP_SDK.Logging.MelonLoaderLogger();
        }
    }
}
