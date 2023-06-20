using CP_SDK.Unity.Extensions;
using MelonLoader;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SynthRidersPlus
{
    /// <summary>
    /// Melon main class
    /// </summary>
    public class Mod : MelonMod
    {
        /// <summary>
        /// Harmony ID for patches
        /// </summary>
        internal static string HarmonyID => "com.github.hardcpp.synthridersplus";

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Harmony patch holder
        /// </summary>
        private HarmonyLib.Harmony m_Harmony;
        /// <summary>
        /// Is initialized
        /// </summary>
        private bool m_Initialized = false;
        /// <summary>
        /// Base game font
        /// </summary>
        private static TMP_FontAsset m_BaseGameFont = null;
        /// <summary>
        /// UI Z-Wrap container
        /// </summary>
        private static GameObject m_ZWrap = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public override void OnInitializeMelon()
        {
            CP_SDK.ChatPlexSDK.Configure(
                new CP_SDK.Logging.MelonLoaderLogger(),
                "SynthRidersPlus",
                Environment.CurrentDirectory,
                CP_SDK.ChatPlexSDK.ERenderPipeline.BuiltIn
            );
            CP_SDK.ChatPlexSDK.OnAssemblyLoaded();

            CP_SDK.Chat.Service.Discrete_OnTextMessageReceived += Service_Discrete_OnTextMessageReceived;

            CP_SDK.Unity.FontManager.Setup(p_FontClone: (p_Input) =>
            {
                var l_MainFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault();
                var l_NewFont  = TMP_FontAsset.CreateFontAsset(p_Input.sourceFontFile);

                l_NewFont.name                      = p_Input.name + " Clone";
                l_NewFont.hashCode                  = TMP_TextUtilities.GetSimpleHashCode(p_Input.name + " Clone" + CP_SDK.Misc.Time.UnixTimeNowMS());
                l_NewFont.fallbackFontAssetTable    = p_Input.fallbackFontAssetTable;

                if (l_MainFont)
                {
                    l_NewFont.material.shader = l_MainFont.material.shader;
                    l_NewFont.material.color  = l_NewFont.material.color.WithAlpha(0.5f);
                    l_NewFont.material.EnableKeyword("UNITY_UI_CLIP_RECT");
                }

                return l_NewFont;
            });

            PatchUI();
        }
        public override void OnApplicationQuit()
        {
            CP_SDK.ChatPlexSDK.StopModules();
            CP_SDK.ChatPlexSDK.OnUnityExit();
            CP_SDK.ChatPlexSDK.OnAssemblyExit();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (m_Initialized)
                return;

            try
            {
                CP_SDK.ChatPlexSDK.OnUnityReady();

                CP_SDK.ChatPlexSDK.Logger.Debug("[SynthRidersPlus][Mod.OnSceneWasLoaded] Applying Harmony patches.");

                /// Setup harmony
                m_Harmony = new HarmonyLib.Harmony(HarmonyID);
                m_Harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

                CP_SDK.ChatPlexSDK.Logger.Debug("[SynthRidersPlus][Mod.OnSceneWasLoaded] Init helpers.");
                SDK.Game.Logic.Init();
                SDK.UI.ModMenuTracker.TouchInstance();

                PatchInput();

                CP_SDK.ChatPlexSDK.InitModules();
            }
            catch (Exception p_Exception)
            {
                CP_SDK.ChatPlexSDK.Logger.Error("[SynthRidersPlus][Mod.OnSceneWasLoaded] Error:");
                CP_SDK.ChatPlexSDK.Logger.Error(p_Exception);
            }

            m_Initialized = true;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On text message received
        /// </summary>
        /// <param name="p_Service">Source service</param>
        /// <param name="p_Message">Message</param>
        private void Service_Discrete_OnTextMessageReceived(CP_SDK.Chat.Interfaces.IChatService p_Service, CP_SDK.Chat.Interfaces.IChatMessage p_Message)
        {
            if (p_Message.Message.Length > 2 && p_Message.Message[0] == '!')
            {
                string l_LMessage = p_Message.Message.ToLower();

                if (l_LMessage.StartsWith("!srplusversion"))
                {
                    p_Service.SendTextMessage(p_Message.Channel,
                        $"! @{p_Message.Sender.DisplayName} The current SR+ version is "
                        + $"{CP_SDK.ChatPlexSDK.ProductVersion}!"
                        + $" The game version is "
                        + $"{Application.version}");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Patch UI system
        /// </summary>
        private void PatchUI()
        {
            CP_SDK.UI.UISystem.FloatingPanelFactory = new SDK.UI.DefaultFactoriesOverrides.SR_FloatingPanelFactory();

            //CP_SDK.UI.UISystem.UILayer      = 29;
            CP_SDK.UI.UISystem.FontScale    = 0.9f;

            CP_SDK.UI.UISystem.Override_GetUIFont = () =>
            {
                if (m_BaseGameFont || CP_SDK.ChatPlexSDK.ActiveGenericScene != CP_SDK.ChatPlexSDK.EGenericScene.Menu) return m_BaseGameFont;
                m_BaseGameFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault();
                m_BaseGameFont.fallbackFontAssetTable.Add(CP_SDK.Unity.FontManager.GetMainFont());

                return m_BaseGameFont;
            };
            CP_SDK.UI.UISystem.Override_OnClick = (p_MonoBehavior) =>
            {
                /// todo
            };

            CP_SDK.UI.UISystem.OnScreenCreated = (x) =>
            {
                if (!x.GetComponent<CP_SDK.XRInput.XRGraphicRaycaster>())
                    x.gameObject.AddComponent<CP_SDK.XRInput.XRGraphicRaycaster>();

                if (!x.GetComponent<SDK.UI.CanvasBlocker>())
                    x.gameObject.AddComponent<SDK.UI.CanvasBlocker>();
            };

            ////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////

            CP_SDK.UI.ScreenSystem.OnCreated += () =>
            {
                var l_Instance = CP_SDK.UI.ScreenSystem.Instance;
                l_Instance.transform.localScale = 1.0f * Vector3.one;
            };
            CP_SDK.UI.ScreenSystem.OnPresent += ScreenSystem_OnPresent;
            CP_SDK.UI.ScreenSystem.OnDismiss += ScreenSystem_OnDismiss;

            ////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////

            CP_SDK.UI.ModMenu.OnCreated += () =>
            {
                var l_Instance = CP_SDK.UI.ModMenu.Instance;
                l_Instance.transform.localScale                 = 0.8f * Vector3.one;
                l_Instance.ScreenContainer.localPosition        = new Vector3(0.00f,   1.90f, 0.53f);
                l_Instance.Screen.RTransform.localEulerAngles   = new Vector3(0.00f, 305.00f, 0.00f);
            };
        }
        /// <summary>
        /// Screen system on present
        /// </summary>
        private void ScreenSystem_OnPresent()
        {
            if (m_ZWrap == null || !m_ZWrap)
                m_ZWrap = GameObject.Find("Z-Wrap");

            if (!m_ZWrap)
                return;

            m_ZWrap.transform.localScale = Vector3.zero;
        }
        /// <summary>
        /// Screen system on dismiss
        /// </summary>
        private void ScreenSystem_OnDismiss()
        {
            if (!m_ZWrap)
                return;

            m_ZWrap.transform.localScale = Vector3.one;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Patch input
        /// </summary>
        private void PatchInput()
        {
            CP_SDK.XRInput.XRLaserPointer.Render = false;
            SDK.XRInput.SRController.LeftController();
            SDK.XRInput.SRController.RightController();
        }
    }
}
