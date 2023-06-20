using Newtonsoft.Json;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace ChatPlexMod_CustomCameras
{
    /// <summary>
    /// Custom camera manager
    /// </summary>
    public class CustomCameras : CP_SDK.ModuleBase<CustomCameras>
    {
        public override CP_SDK.EIModuleBaseType             Type            => CP_SDK.EIModuleBaseType.Integrated;
        public override string                              Name            => "Custom Cameras";
        public override string                              Description     => "Make your own POVs!";
        public override bool                                UseChatFeatures => false;
        public override bool                                IsEnabled       { get => CCConfig.Instance.Enabled; set => CCConfig.Instance.Enabled = value; }
        public override CP_SDK.EIModuleBaseActivationType   ActivationType  => CP_SDK. EIModuleBaseActivationType.OnMenuSceneLoaded;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private UI.SettingsMainView m_SettingsMainView = null;

        private string m_Folder         = "";
        private string m_ConfigFileName = "";
        private string m_CamerasFolder  = "";

        private FileSystemWatcher   m_Watcher               = null;
        private bool                m_Changed               = false;
        private int                 m_ChangedIgnoreCount    = 0;

        private Models.Config                               m_Config            = new Models.Config();
        private Dictionary<string, Components.CustomCamera> m_CustomCameras     = new Dictionary<string, Components.CustomCamera>();
        private Transform                                   m_RoomCorrection    = null;

        private bool m_IsFirstMenuLoading = true;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Room correction transform component accessor
        /// </summary>
        internal Transform RoomCorrection => m_RoomCorrection;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Enable the Module
        /// </summary>
        protected override void OnEnable()
        {
            m_Folder            = Path.Combine("UserData", CP_SDK.ChatPlexSDK.ProductName, "CustomCameras");
            m_ConfigFileName    = Path.Combine(m_Folder, "Scenes.json");
            m_CamerasFolder     = Path.Combine(m_Folder, "Cameras");

            try
            {
                if (!Directory.Exists(m_Folder))
                    Directory.CreateDirectory(m_Folder);
            }
            catch (System.Exception l_Exception)
            {
                Logger.Instance.Error($"[ChatPlexMod_CustomCameras][CustomCameras.Start] Creating folder \"{m_Folder}\"");
                Logger.Instance.Error(l_Exception);
            }

            try
            {
                if (!File.Exists(m_ConfigFileName))
                    SaveConfig();
                else
                {
                    var l_Content = File.ReadAllText(m_ConfigFileName, Encoding.UTF8);
                    JsonConvert.PopulateObject(l_Content, m_Config);

                    /// Save in case of data migration
                    SaveConfig();
                }
            }
            catch (System.Exception l_Exception)
            {
                Logger.Instance.Error($"[ChatPlexMod_CustomCameras][CustomCameras.Start] Reading configuration file \"{m_ConfigFileName}\"");
                Logger.Instance.Error(l_Exception);
            }

            try
            {
                if (m_Watcher == null)
                {
                    m_Watcher = new FileSystemWatcher();
                    m_Watcher.Path          = Path.GetDirectoryName(m_ConfigFileName);
                    m_Watcher.Filter        = Path.GetFileName(m_ConfigFileName);
                    m_Watcher.NotifyFilter  = NotifyFilters.LastWrite | NotifyFilters.Size;
                    m_Watcher.Changed       += OnFileChanged;
                    m_Watcher.EnableRaisingEvents = true;

                    CP_SDK.Unity.MTCoroutineStarter.Start(Coroutine_UpdateWatcher());
                }
            }
            catch (System.Exception l_Exception)
            {
                Logger.Instance.Error($"[ChatPlexMod_CustomCameras][CustomCameras.Start] Error configuring watcher for file \"{m_ConfigFileName}\"");
                Logger.Instance.Error(l_Exception);
            }

            try
            {
                if (!Directory.Exists(m_CamerasFolder))
                {
                    Directory.CreateDirectory(m_CamerasFolder);
                    SaveCameraExample();
                }
            }
            catch (System.Exception l_Exception)
            {
                Logger.Instance.Error($"[ChatPlexMod_CustomCameras][CustomCameras.Start] Creating folder \"{m_CamerasFolder}\"");
                Logger.Instance.Error(l_Exception);
            }


            CP_SDK.ChatPlexSDK.OnGenericSceneChange+= ChatPlexSDK_OnGenericSceneChange;

            m_RoomCorrection = new GameObject("CPM_CC_RoomCorection").transform;
            m_RoomCorrection.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            GameObject.DontDestroyOnLoad(m_RoomCorrection.gameObject);
        }
        /// <summary>
        /// Disable the Module
        /// </summary>
        protected override void OnDisable()
        {
            if (m_Watcher != null)
            {
                m_Watcher.Changed -= OnFileChanged;
                m_Watcher.Dispose();
                m_Watcher = null;
            }

            SaveConfig();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get Module settings UI
        /// </summary>
        protected override (CP_SDK.UI.IViewController, CP_SDK.UI.IViewController, CP_SDK.UI.IViewController) GetSettingsViewControllersImplementation()
        {
            if (m_SettingsMainView == null) m_SettingsMainView = CP_SDK.UI.UISystem.CreateViewController<UI.SettingsMainView>();

            return (m_SettingsMainView, null, null);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On game state changedf
        /// </summary>
        /// <param name="p_Sender">Event sender</param>
        /// <param name="p_GenericScene">New game state</param>
        private void ChatPlexSDK_OnGenericSceneChange(CP_SDK.ChatPlexSDK.EGenericScene p_GenericScene)
        {
            if (m_IsFirstMenuLoading)
            {
                CP_SDK.Unity.MTCoroutineStarter.Start(Coroutine_FirstLoading(p_GenericScene));
                return;
            }

            var l_TargetList = null as List<string>;

            switch (p_GenericScene)
            {
                case CP_SDK.ChatPlexSDK.EGenericScene.Menu:
                    l_TargetList = m_Config.MenuCameras;
                    break;

                case CP_SDK.ChatPlexSDK.EGenericScene.Playing:
                    l_TargetList = m_Config.GameplayCameras;
                    break;
            }

            foreach (var l_KVP in m_CustomCameras)
                l_KVP.Value.SetCameraEnabled(l_TargetList != null && l_TargetList.Contains(l_KVP.Key));

            CP_SDK.Unity.MTCoroutineStarter.Start(Coroutine_FixRoomCorrection());
        }
        /// <summary>
        /// First loading coroutine
        /// </summary>
        /// <param name="p_GameState"></param>
        /// <returns></returns>
        private IEnumerator Coroutine_FirstLoading(CP_SDK.ChatPlexSDK.EGenericScene p_GameState)
        {
            m_IsFirstMenuLoading = false;

#if WALKABOUTMINIGOLF || AUDIOTRIP
            yield return null;
#elif SYNTHRIDERS
            yield return new WaitUntil(() => GameObject.FindObjectsOfType<Camera>().LastOrDefault(x => x.gameObject.activeInHierarchy && x.gameObject.name == "Cam") != null);
#else
#error Missing game implementation
#endif
            ProcessConfig();
            ChatPlexSDK_OnGenericSceneChange(p_GameState);
        }
        /// <summary>
        /// Fix room correction coroutine
        /// </summary>
        /// <returns></returns>
        private IEnumerator Coroutine_FixRoomCorrection()
        {
            yield return new WaitForSecondsRealtime(0.1f);

#if WALKABOUTMINIGOLF || AUDIOTRIP
            ;
#elif SYNTHRIDERS
            yield return new WaitUntil(() => GameObject.FindObjectsOfType<VRTK.VRTK_SDKSetup>().FirstOrDefault(x => x.isActiveAndEnabled));

            var l_Room = GameObject.FindObjectsOfType<VRTK.VRTK_SDKSetup>().FirstOrDefault(x => x.isActiveAndEnabled);

            m_RoomCorrection.SetPositionAndRotation(l_Room.transform.position, l_Room.transform.rotation);
#else
#error Missing game implementation
#endif
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Coroutine update watcher
        /// </summary>
        /// <returns></returns>
        private IEnumerator Coroutine_UpdateWatcher()
        {
            var l_Waiter = new WaitUntil(() => m_Changed);

            while (true)
            {
                yield return l_Waiter;

                if (m_Changed)
                {
                    try
                    {
                        using (var l_FileStream = new System.IO.FileStream(m_ConfigFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                        {
                            using (var l_StreamReader = new System.IO.StreamReader(l_FileStream, Encoding.UTF8))
                            {
                                var l_Content = l_StreamReader.ReadToEnd();
                                m_Config = new Models.Config();
                                JsonConvert.PopulateObject(l_Content, m_Config);
                            }
                        }

                        ProcessConfig();
                    }
                    catch (System.Exception l_Exception)
                    {
                        Logger.Instance.Error($"[ChatPlexMod_CustomCameras][CustomCameras.Coroutine_UpdateWatcher] Error loading file \"{m_ConfigFileName}\"");
                        Logger.Instance.Error(l_Exception);
                    }

                    m_Changed = false;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Process new configuration
        /// </summary>
        private void ProcessConfig()
        {
            var l_CamerasToLoad = m_Config.MenuCameras.Concat(m_Config.GameplayCameras).Distinct();

            foreach (var l_CameraName in l_CamerasToLoad)
            {
                try
                {
                    var l_Path = Path.Combine(m_CamerasFolder, l_CameraName + ".json");

                    if (!File.Exists(l_Path))
                        SaveCameraExample(l_CameraName);

                    var l_Content = File.ReadAllText(l_Path, Encoding.UTF8);

                    if (m_CustomCameras.TryGetValue(l_CameraName, out var l_ExistingCamera))
                        l_ExistingCamera.Deserialize(l_Path, l_Content);
                    else
                    {
#if WALKABOUTMINIGOLF
                        var l_Base = GameObject.Instantiate(GameObject.FindObjectsOfType<Camera>().LastOrDefault(x => x.gameObject.activeInHierarchy && x.gameObject.name == "Main Eye").gameObject, m_RoomCorrection);
#elif SYNTHRIDERS
                        var l_Base = GameObject.Instantiate(GameObject.FindObjectsOfType<BeautifyEffect.Beautify>().FirstOrDefault(x => x.gameObject.activeInHierarchy && x.GetComponent<Camera>()).gameObject, m_RoomCorrection);
#elif AUDIOTRIP
                        var l_Base = GameObject.Instantiate(Camera.main, null);
#else
                        #error Missing game implementation
#endif
                        l_Base.name = "CustomCamera_" + l_CameraName;
                        l_Base.tag  = "Untagged";

#if WALKABOUTMINIGOLF || AUDIOTRIP
                        ;
#elif SYNTHRIDERS
                        if (l_Base.GetComponent<BeautifyEffect.Beautify>())
                            l_Base.GetComponent<BeautifyEffect.Beautify>().bloomIntensity = 0.15f;
#else
                        #error Missing game implementation
#endif

                        while (l_Base.transform.childCount > 0)
                            GameObject.DestroyImmediate(l_Base.transform.GetChild(0).gameObject);

                        foreach (var l_Component in l_Base.GetComponents<Object>())
                        {
                            if (l_Component.GetType() != typeof(Transform)
                                && l_Component.GetType() != typeof(Camera)
#if WALKABOUTMINIGOLF

#elif SYNTHRIDERS
                                && l_Component.GetType() != typeof(UnityEngine.Rendering.PostProcessing.PostProcessLayer)
                                && l_Component.GetType() != typeof(BeautifyEffect.Beautify)
#elif AUDIOTRIP
                                && l_Component.GetType() != typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData)
                                && l_Component.GetType() != typeof(UnityEngine.Rendering.PostProcessing.PostProcessLayer)
#else
#error Missing game implementation
#endif
                                 )
                                GameObject.DestroyImmediate(l_Component);
                        }

                        var l_NewCamera = l_Base.gameObject.AddComponent<Components.CustomCamera>();
                        l_NewCamera.Deserialize(l_Path, l_Content);

                        m_CustomCameras.Add(l_CameraName, l_NewCamera);
                    }
                }
                catch (System.Exception l_Exception)
                {
                    Logger.Instance.Error($"[ChatPlexMod_CustomCameras][CustomCameras.ProcessConfig] Error reading camera \"{l_CameraName}\"");
                    Logger.Instance.Error(l_Exception);
                }
            }

            var l_ExistingList = new List<string>(m_CustomCameras.Keys.ToList());
            foreach (var l_CameraName in l_ExistingList)
            {
                if (l_CamerasToLoad.Contains(l_CameraName))
                    continue;

                if (m_CustomCameras.TryGetValue(l_CameraName, out var l_ToDelete))
                {
                    m_CustomCameras.Remove(l_CameraName);
                    GameObject.Destroy(l_ToDelete.gameObject);
                }
            }

            ChatPlexSDK_OnGenericSceneChange(CP_SDK.ChatPlexSDK.ActiveGenericScene);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On file changed
        /// </summary>
        /// <param name="p_Sender">Event sender</param>
        /// <param name="p_Event">Event data</param>
        private void OnFileChanged(object p_Sender, FileSystemEventArgs p_Event)
        {
            if (m_ChangedIgnoreCount > 0)
            {
                m_ChangedIgnoreCount--;
                return;
            }

            m_Changed = true;
        }
        /// <summary>
        /// Save config file
        /// </summary>
        private void SaveConfig()
        {
            if (m_Watcher != null)
                m_ChangedIgnoreCount++;

            try
            {
                using (var l_FileStream = new System.IO.FileStream(m_ConfigFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite))
                {
                    using (var l_StreamWritter = new System.IO.StreamWriter(l_FileStream, Encoding.UTF8))
                    {
                        l_StreamWritter.WriteLine(JsonConvert.SerializeObject(m_Config, Formatting.Indented));
                    }
                }
#if DEBUG
                Logger.Instance.Warning($"[ChatPlexMod_CustomCameras][CustomCameras.SaveConfig] Saving to file \"{m_ConfigFileName}\"");
#endif
            }
            catch (System.Exception l_Exception)
            {
                Logger.Instance.Error($"[ChatPlexMod_CustomCameras][CustomCameras.SaveConfig] Error saving to file \"{m_ConfigFileName}\"");
                Logger.Instance.Error(l_Exception);
            }
        }
        /// <summary>
        /// Save a template camera
        /// </summary>
        private void SaveCameraExample(string p_Name = "Default")
        {
            try
            {
                var l_DefaultCamera = new Models.Camera();
                File.WriteAllText(Path.Combine(m_CamerasFolder, p_Name + ".json"), JsonConvert.SerializeObject(l_DefaultCamera, Formatting.Indented), Encoding.UTF8);
            }
            catch (System.Exception l_Exception)
            {
                Logger.Instance.Error($"[ChatPlexMod_CustomCameras][CustomCameras.SaveCameraExample] Error saving camera \"{p_Name}\"");
                Logger.Instance.Error(l_Exception);
            }
        }
    }
}