using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;

namespace ChatPlexMod_CustomCameras.Components
{
    /// <summary>
    /// Custom camera instance
    /// </summary>
    internal class CustomCamera : MonoBehaviour
    {
        /// <summary>
        /// Camera config path
        /// </summary>
        private string m_Path = "";
        /// <summary>
        /// Main camera cache
        /// </summary>
        private Camera m_MainCamera = null;
        /// <summary>
        /// Camera component
        /// </summary>
        private Camera m_CameraComponent = null;
        /// <summary>
        /// Camera config
        /// </summary>
        private Models.Camera m_CameraConfig = new Models.Camera();
        /// <summary>
        /// File watcher
        /// </summary>
        private FileSystemWatcher m_Watcher = null;
        /// <summary>
        /// Changed flag
        /// </summary>
        private bool m_Changed = false;
        /// <summary>
        /// Changed ignore count
        /// </summary>
        private int m_ChangedIgnoreCount = 0;
        /// <summary>
        /// Smooth coroutine instance
        /// </summary>
        private Coroutine m_SmoothCoroutine = null;
        /// <summary>
        /// Dynamic Near Far coroutine instance
        /// </summary>
        private Coroutine m_DynamicNearFarCoroutine = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On component first frame
        /// </summary>
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
        }
        /// <summary>
        /// On object destroy
        /// </summary>
        private void OnDestroy()
        {
            if (m_Watcher != null)
            {
                m_Watcher.Changed -= OnFileChanged;
                m_Watcher.Dispose();
                m_Watcher = null;
            }

            SaveCameraConfig();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Deserialize Camera from raw Json
        /// </summary>
        /// <param name="p_Path">File path</param>
        /// <param name="p_Content">Json content</param>
        internal void Deserialize(string p_Path, string p_Content)
        {
            m_Path = p_Path;

            if (!m_MainCamera)
                m_MainCamera = Camera.main;

            if (!m_CameraComponent)
            {
                m_CameraComponent = GetComponent<Camera>();
                m_CameraComponent.stereoTargetEye   = StereoTargetEyeMask.None;
                m_CameraComponent.targetDisplay     = 0;
                m_CameraComponent.enabled           = false;
            }

            try
            {
                m_CameraConfig = new Models.Camera();
                JsonConvert.PopulateObject(p_Content, m_CameraConfig);
                m_CameraConfig.Validate();

                if (m_Watcher == null)
                    SaveCameraConfig();
            }
            catch (System.Exception l_Exception)
            {
                Logger.Instance.Error($"[CustomCameras][CustomCamera.Deserialize] Error loading file \"{m_Path}\"");
                Logger.Instance.Error(l_Exception);

                SaveCameraConfig();
            }

            try
            {
                if (m_Watcher == null)
                {
                    m_Watcher = new FileSystemWatcher();
                    m_Watcher.Path          = Path.GetDirectoryName(m_Path);
                    m_Watcher.Filter        = Path.GetFileName(m_Path);
                    m_Watcher.NotifyFilter  = NotifyFilters.LastWrite | NotifyFilters.Size;
                    m_Watcher.Changed       += OnFileChanged;
                    m_Watcher.EnableRaisingEvents = true;

                    StartCoroutine(Coroutine_UpdateWatcher());
                }
            }
            catch (System.Exception l_Exception)
            {
                Logger.Instance.Error($"[CustomCameras][CustomCamera.Deserialize] Error configuring watcher for file \"{m_Path}\"");
                Logger.Instance.Error(l_Exception);
            }

            if (m_SmoothCoroutine != null)
            {
                StopCoroutine(m_SmoothCoroutine);
                m_SmoothCoroutine = null;
            }

            if (m_DynamicNearFarCoroutine != null)
            {
                StopCoroutine(m_DynamicNearFarCoroutine);
                m_DynamicNearFarCoroutine = null;
            }

            if (m_CameraConfig.IsFirstPerson && !m_CameraConfig.SmoothCamera && m_MainCamera)
                transform.SetParent(m_MainCamera.transform, true);
            else
                transform.SetParent(CustomCameras.Instance.RoomCorrection, true);

            if (m_CameraConfig.IsFirstPerson && m_CameraConfig.SmoothCamera)
                m_SmoothCoroutine = StartCoroutine(Coroutine_SmoothCamera());

            if (m_CameraConfig.RelativeNearFarOverride.Enabled)
                m_DynamicNearFarCoroutine = StartCoroutine(Coroutine_DynamicNearFar());

            transform.localPosition     = new Vector3(m_CameraConfig.Position.X, m_CameraConfig.Position.Y, m_CameraConfig.Position.Z);
            transform.localEulerAngles  = new Vector3(m_CameraConfig.Rotation.X, m_CameraConfig.Rotation.Y, m_CameraConfig.Rotation.Z);

            if (m_CameraConfig.IsFirstPerson && m_CameraConfig.SmoothCamera && m_MainCamera)
            {
                var l_PositionOffset = new Vector3(m_CameraConfig.Position.X, m_CameraConfig.Position.Y, m_CameraConfig.Position.Z);
                var l_RotationOffset = Quaternion.Euler(m_CameraConfig.Rotation.X, m_CameraConfig.Rotation.Y, m_CameraConfig.Rotation.Z);

                transform.position = m_MainCamera.transform.position + l_PositionOffset;
                transform.rotation = m_MainCamera.transform.rotation * l_RotationOffset;
            }

            m_CameraComponent.fieldOfView   = m_CameraConfig.FOV;
            m_CameraComponent.rect          = new Rect(new Vector2(m_CameraConfig.Rect.PosX, m_CameraConfig.Rect.PosY), new Vector2(m_CameraConfig.Rect.Width, m_CameraConfig.Rect.Height));
            m_CameraComponent.depth         = m_CameraConfig.Layer;
            m_CameraComponent.cullingMask   = m_CameraConfig.LayerMask;
            m_CameraComponent.nearClipPlane = 0.1f;
            m_CameraComponent.farClipPlane  = 6000;

            if (m_CameraConfig.Skybox)
                m_CameraComponent.clearFlags = CameraClearFlags.Skybox;
            else
            {
                m_CameraComponent.clearFlags        = CameraClearFlags.SolidColor;
                m_CameraComponent.backgroundColor   = m_CameraConfig.NoSkyboxTransparent ? Color.clear : Color.black;
            }

#if WALKABOUTMINIGOLF || AUDIOTRIP
            ;
#elif SYNTHRIDERS
            if (GetComponent<BeautifyEffect.Beautify>())
                GetComponent<BeautifyEffect.Beautify>().bloomIntensity = m_CameraConfig.BloomIntensity;
#else
            #error Missing game implementation
#endif
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set if the camera is enabled or not
        /// </summary>
        /// <param name="p_Enabled"></param>
        internal void SetCameraEnabled(bool p_Enabled)
        {
            if (m_CameraComponent)
                m_CameraComponent.enabled = p_Enabled;
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
                        using (var l_FileStream = new System.IO.FileStream(m_Path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                        {
                            using (var l_StreamReader = new System.IO.StreamReader(l_FileStream, Encoding.UTF8))
                            {
                                var l_Content = l_StreamReader.ReadToEnd();
                                Deserialize(m_Path, l_Content);
                                m_CameraConfig.Validate();
                            }
                        }
                    }
                    catch (System.Exception l_Exception)
                    {
                        Logger.Instance.Error($"[CustomCameras][CustomCamera.Coroutine_UpdateWatcher] Error loading file \"{m_Path}\"");
                        Logger.Instance.Error(l_Exception);
                    }

                    m_Changed = false;
                }
            }
        }
        /// <summary>
        /// Smooth camera coroutine
        /// </summary>
        /// <returns></returns>
        private IEnumerator Coroutine_SmoothCamera()
        {
            var l_Waiter = new WaitForEndOfFrame();

            var l_PositionOffset = new Vector3(m_CameraConfig.Position.X, m_CameraConfig.Position.Y, m_CameraConfig.Position.Z);
            var l_RotationOffset = Quaternion.Euler(m_CameraConfig.Rotation.X, m_CameraConfig.Rotation.Y, m_CameraConfig.Rotation.Z);

            while (m_CameraConfig.SmoothCamera)
            {
                if (!m_MainCamera)
                    m_MainCamera = Camera.main;

                if (m_MainCamera)
                {
                    transform.position = Vector3.Lerp(transform.position, m_MainCamera.transform.position + l_PositionOffset, Time.smoothDeltaTime * 5f * (1f / m_CameraConfig.SmoothingFactor));
                    transform.rotation = Quaternion.Lerp(transform.rotation, m_MainCamera.transform.rotation * l_RotationOffset, Time.smoothDeltaTime * 5f * (1f / m_CameraConfig.SmoothingFactor));
                }

                yield return l_Waiter;
            }

            yield return null;
        }
        /// <summary>
        /// On frame
        /// </summary>
        private IEnumerator Coroutine_DynamicNearFar()
        {
            var l_Waiter = new WaitForEndOfFrame();

            while (m_CameraConfig.RelativeNearFarOverride.Enabled)
            {
                if (!m_MainCamera)
                    m_MainCamera = Camera.main;

                if (m_CameraComponent && m_MainCamera && m_CameraConfig.RelativeNearFarOverride.Enabled)
                {
                    var l_Override = m_CameraConfig.RelativeNearFarOverride;

                    m_CameraComponent.nearClipPlane = Mathf.Max(0.1f, l_Override.RelNear + m_MainCamera.transform.position.z);
                    m_CameraComponent.farClipPlane  = l_Override.RelFar + m_MainCamera.transform.position.z;
                }

                yield return l_Waiter;
            }

            yield return null;
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
        /// Save camera config file
        /// </summary>
        private void SaveCameraConfig()
        {
            if (m_Watcher != null)
                m_ChangedIgnoreCount++;

            m_CameraConfig.Validate();

            try
            {
                if (string.IsNullOrEmpty(m_Path))
                    return;

                using (var l_FileStream = new System.IO.FileStream(m_Path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite))
                {
                    using (var l_StreamWritter = new System.IO.StreamWriter(l_FileStream, Encoding.UTF8))
                    {
                        l_StreamWritter.WriteLine(JsonConvert.SerializeObject(m_CameraConfig, Formatting.Indented));
                    }
                }
#if UNITY_EDITOR
                Debug.LogWarning($"[CustomCameras][CustomCamera.SaveCameraConfig] Saving to file \"{m_Path}\"");
#endif
            }
            catch (System.Exception l_Exception)
            {
                Logger.Instance.Error($"[CustomCameras][CustomCamera.SaveCameraConfig] Error saving to file \"{m_Path}\"");
                Logger.Instance.Error(l_Exception);
            }
        }
    }
}