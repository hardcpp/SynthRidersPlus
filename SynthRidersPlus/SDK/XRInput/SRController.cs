using UnityEngine;

namespace SynthRidersPlus.SDK.XRInput
{
    /// <summary>
    /// SynthRiders controller
    /// </summary>
    internal class SRController : CP_SDK.XRInput.XRController
    {
        private static SRController m_LeftController;
        private static SRController m_RightController;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private Transform GameTransform;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Left controller instance
        /// </summary>
        /// <returns></returns>
        internal static SRController LeftController()
        {
            if (!m_LeftController)
            {
                m_LeftController = new GameObject("[SynthRidersPlus.SDK.XRInput.SRController]").AddComponent<SRController>();
                m_LeftController.IsLeftHand     = true;
                m_LeftController.RawTransform   = m_LeftController.transform;
                GameObject.DontDestroyOnLoad(m_LeftController.gameObject);
            }
            return m_LeftController;
        }
        /// <summary>
        /// Right controller instance
        /// </summary>
        /// <returns></returns>
        internal static SRController RightController()
        {
            if (!m_RightController)
            {
                m_RightController = new GameObject("[SynthRidersPlus.SDK.XRInput.SRController]").AddComponent<SRController>();
                m_RightController.IsLeftHand    = false;
                m_RightController.RawTransform  = m_RightController.transform;
                GameObject.DontDestroyOnLoad(m_RightController.gameObject);

                CP_SDK.XRInput.XRInputSystem.SetRaycastingController(m_RightController);
            }
            return m_RightController;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Component first frame
        /// </summary>
        private void Awake()
        {
            Game.Logic.OnSceneChange += Logic_OnSceneChange;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On frame
        /// </summary>
        private void Update()
        {
            if (!GameTransform)
                return;

            transform.SetPositionAndRotation(GameTransform.position, GameTransform.rotation);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On scene change
        /// </summary>
        /// <param name="p_Scene">New scene</param>
        private void Logic_OnSceneChange(Game.Logic.SceneType p_Scene)
        {
            CP_SDK.XRInput.XRInputSystem.TouchInstance();
            GameTransform = null;
            FindBaseGameTransform();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Find base game transform
        /// </summary>
        private void FindBaseGameTransform()
        {
            if (GameTransform)
                return;

            var l_IsLeftHand = IsLeftHand;
            var l_Attempt1 = "/VRTKManager/LatencyWrap/[Oculus]/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerAnchor";
            var l_Attempt2 = "/VRTKManager/LatencyWrap/SteamVRSetup/[CameraRig]/Controller (right)/Model/tip/attach";

            if (l_IsLeftHand)
            {
                l_Attempt1 = "/VRTKManager/LatencyWrap/[Oculus]/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerAnchor";
                l_Attempt2 = "/VRTKManager/LatencyWrap/SteamVRSetup/[CameraRig]/Controller (left)/Model/tip/attach";
            }

            var l_GameObject = GameObject.Find(l_Attempt1);
            if (!l_GameObject || !l_GameObject.activeInHierarchy)
                l_GameObject = GameObject.Find(l_Attempt2);

            GameTransform = l_GameObject?.transform;

            if (GameTransform && CP_SDK.XRInput.XRLaserPointer.Instance.CurrentController == this)
                CP_SDK.XRInput.XRLaserPointer.Instance.SetController(this);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On joystick change
        /// </summary>
        /// <param name="_"></param>
        /// <param name="p_Event">Event data</param>
        public static void ControllerEvents_TouchpadAxisChanged(object _, VRTK.ControllerInteractionEventArgs p_Event)
        {
            if (p_Event.controllerReference == null)
                return;

            var l_Target = p_Event.controllerReference.hand == VRTK.SDK_BaseController.ControllerHand.Left ? m_LeftController : m_RightController;
            if (!l_Target)
                return;

            l_Target.FindBaseGameTransform();

            if (p_Event.touchpadAxis.magnitude >= 0.10f)
                l_Target.Axis0Joystick = p_Event.touchpadAxis * -1.0f;
            else
                l_Target.Axis0Joystick = Vector2.zero;
        }
        /// <summary>
        /// On trigger press
        /// </summary>
        /// <param name="_"></param>
        /// <param name="p_Event">Event data</param>
        public static void ControllerEvents_TriggerClicked(object _, VRTK.ControllerInteractionEventArgs p_Event)
        {
            if (p_Event.controllerReference == null)
                return;

            var l_Target = p_Event.controllerReference.hand == VRTK.SDK_BaseController.ControllerHand.Left ? m_LeftController : m_RightController;
            if (!l_Target)
                return;

            l_Target.FindBaseGameTransform();

            l_Target.m_PressButtonData[(int)CP_SDK.XRInput.EXRInputButton.Axis1Button] = true;
            CP_SDK.XRInput.XRInputSystem.OnControllerButtonPressData(l_Target, CP_SDK.XRInput.EXRInputButton.Axis1Button, true);
        }
        /// <summary>
        /// On trigger release
        /// </summary>
        /// <param name="_"></param>
        /// <param name="p_Event">Event data</param>
        public static void ControllerEvents_TriggerUnclicked(object _, VRTK.ControllerInteractionEventArgs p_Event)
        {
            if (p_Event.controllerReference == null)
                return;

            var l_Target = p_Event.controllerReference.hand == VRTK.SDK_BaseController.ControllerHand.Left ? m_LeftController : m_RightController;
            if (!l_Target)
                return;

            l_Target.FindBaseGameTransform();

            l_Target.m_PressButtonData[(int)CP_SDK.XRInput.EXRInputButton.Axis1Button] = false;
            CP_SDK.XRInput.XRInputSystem.OnControllerButtonPressData(l_Target, CP_SDK.XRInput.EXRInputButton.Axis1Button, false);
        }
        /// <summary>
        /// On trigger axis changed
        /// </summary>
        /// <param name="_"></param>
        /// <param name="p_Event">Event data</param>
        public static void ControllerEvents_TriggerAxisChanged(object _, VRTK.ControllerInteractionEventArgs p_Event)
        {
            if (p_Event.controllerReference == null)
                return;

            var l_Target = p_Event.controllerReference.hand == VRTK.SDK_BaseController.ControllerHand.Left ? m_LeftController : m_RightController;
            if (!l_Target)
                return;

            l_Target.FindBaseGameTransform();

            l_Target.Axis1Trigger = p_Event.buttonPressure;
        }
    }
}
