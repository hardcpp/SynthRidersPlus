using System;
using System.Collections.Generic;
using System.Linq;

namespace SynthRidersPlus.SDK.Game
{
    /// <summary>
    /// Game helper
    /// </summary>
    public class Logic
    {
        /// <summary>
        /// Scenes
        /// </summary>
        public enum SceneType
        {
            None,
            Menu,
            Playing
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Was the menu scene loaded
        /// </summary>
        private static bool m_MenuWasLoaded = false;
        /// <summary>
        /// Play stage list
        /// </summary>
        private static List<Synth.Data.StageModel> m_StagesList = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Active scene type
        /// </summary>
        public static SceneType ActiveScene { get; private set; } = SceneType.Menu;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On scene change
        /// </summary>
        public static event Action<SceneType> OnSceneChange;
        /// <summary>
        /// On menu scene loaded
        /// </summary>
        public static event Action OnMenuSceneLoaded;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Init
        /// </summary>
        internal static void Init()
        {
#if DEBUG_SCENES || DEBUG
            CP_SDK.ChatPlexSDK.Logger.Error($"====== [SDK.Game][Logic.Init] {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} ======");
#endif

            UnityEngine.SceneManagement.SceneManager.sceneLoaded        += SceneManager_sceneLoaded;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Scene loaded
        /// </summary>
        /// <param name="p_Scene">Loaded scene</param>
        /// <param name="p_Mode">Loading mode</param>
        private static void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene p_Scene, UnityEngine.SceneManagement.LoadSceneMode p_Mode)
        {
#if DEBUG_SCENES || DEBUG
            CP_SDK.ChatPlexSDK.Logger.Error($"====== [SDK.Game][Logic.SceneManager_activeSceneChanged] {p_Scene.name} ======");
#endif

            try
            {
                UpdateStageList();
            }
            catch (Exception l_Exception)
            {
                CP_SDK.ChatPlexSDK.Logger.Error("[SDK.Game][Logic.SceneManager_sceneLoaded] Error:");
                CP_SDK.ChatPlexSDK.Logger.Error(l_Exception.ToString());
            }
        }
        /// <summary>
        /// On active scene changed
        /// </summary>
        /// <param name="p_Current">Current scene</param>
        /// <param name="p_Next">Next scene</param>
        private static void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene p_Current, UnityEngine.SceneManagement.Scene p_Next)
        {
#if DEBUG_SCENES || DEBUG
            CP_SDK.ChatPlexSDK.Logger.Error($"====== [SDK.Game][Logic.SceneManager_activeSceneChanged] {p_Next.name} ======");
#endif

            try
            {
                if (!m_MenuWasLoaded)
                {
                    var l_SynthsFinder = SynthsFinder.s_instance;
                    if (l_SynthsFinder != null && l_SynthsFinder && l_SynthsFinder.MainStageAvailable.Contains(p_Next.name))
                    {
                        ActiveScene = SceneType.Menu;

                        CP_SDK.ChatPlexSDK.Fire_OnGenericMenuSceneLoaded();
                        CP_SDK.ChatPlexSDK.Fire_OnGenericMenuScene();

                        OnMenuSceneLoaded?.Invoke();

                        m_MenuWasLoaded = true;

                        OnSceneChange?.Invoke(ActiveScene);
                    }
                }

                UpdateStageList();

                if (m_StagesList == null)
                    return;

                var l_NewKind = m_StagesList.Count(x => x.scene == p_Next.name) > 0 ? SceneType.Playing : SceneType.Menu;

                ActiveScene = l_NewKind;

                if (l_NewKind == SceneType.Menu)
                    CP_SDK.ChatPlexSDK.Fire_OnGenericMenuScene();
                else
                    CP_SDK.ChatPlexSDK.Fire_OnGenericPlayingScene();

                OnSceneChange?.Invoke(l_NewKind);
            }
            catch (Exception l_Exception)
            {
                CP_SDK.ChatPlexSDK.Logger.Error("[SDK.Game][Logic.SceneManager_activeSceneChanged] Error:");
                CP_SDK.ChatPlexSDK.Logger.Error(l_Exception.ToString());
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Update stage list
        /// </summary>
        private static void UpdateStageList()
        {
            var l_StageProvider = Synth.Data.StagesProvider.s_instance;

            if (l_StageProvider != null && !l_StageProvider.StagesListInited)
                l_StageProvider.InitStagesProvider();

            if (l_StageProvider != null && l_StageProvider && (m_StagesList == null || m_StagesList.Count != l_StageProvider.StagesList.Count))
                m_StagesList = new List<Synth.Data.StageModel>(l_StageProvider.StagesList);
        }
    }
}
