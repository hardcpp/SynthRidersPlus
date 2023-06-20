using UnityEngine;

namespace SynthRidersPlus.SDK.UI.DefaultComponentsOverrides.Subs
{
    /// <summary>
    /// Floating panel mover
    /// </summary>
    internal class SubFloatingPanelMover : MonoBehaviour
    {
        protected const float MinScrollDistance = 0.25f;
        protected const float MaxLaserDistance  = 50f;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private SR_CFloatingPanel           m_FloatingPanel;
        private CP_SDK.XRInput.XRController m_GrabbingController;
        private Vector3                     m_GrabPosition;
        private Quaternion                  m_GrabRotation;
        private RaycastHit[]                m_RaycastBuffer = new RaycastHit[10];

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On component destroy
        /// </summary>
        private void OnDestroy()
        {
            m_FloatingPanel         = null;
            m_GrabbingController    = null;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On frame
        /// </summary>
        private void Update()
        {
            var l_XRInputSystem = CP_SDK.XRInput.XRInputSystem.Instance;
            if (!l_XRInputSystem || !CP_SDK.XRInput.XRInputSystem.EnableRaycasting)
                return;

            var l_Controller = CP_SDK.XRInput.XRInputSystem.RaycastingController;
            if (l_Controller && l_Controller.IsButtonPressed(CP_SDK.XRInput.EXRInputButton.Axis1Button))
            {
                if (m_GrabbingController != null)
                    return;

                var l_HitCount = Physics.RaycastNonAlloc(   l_Controller.RawTransform.position,
                                                            l_Controller.RawTransform.forward,
                                                            m_RaycastBuffer,
                                                            MaxLaserDistance,
                                                            1 << CP_SDK.UI.UISystem.UILayer);

                for (var l_I = 0; l_I < l_HitCount; ++l_I)
                {
                    var l_SubFloatingPanelMoverHandle = m_RaycastBuffer[l_I].transform?.GetComponent<SubFloatingPanelMoverHandle>();
                    if (!l_SubFloatingPanelMoverHandle)
                        continue;

                    m_FloatingPanel         = l_SubFloatingPanelMoverHandle.FloatingPanel;
                    m_GrabbingController    = l_Controller;
                    m_GrabPosition          = l_Controller.RawTransform.InverseTransformPoint(m_FloatingPanel.RTransform.position);
                    m_GrabRotation          = Quaternion.Inverse(l_Controller.RawTransform.rotation) * m_FloatingPanel.RTransform.rotation;

                    m_FloatingPanel.FireOnGrab();
                    break;
                }
            }

            if (m_GrabbingController != null && !l_Controller || !l_Controller.IsButtonPressed(CP_SDK.XRInput.EXRInputButton.Axis1Button))
            {
                m_FloatingPanel.FireOnRelease();

                m_FloatingPanel         = null;
                m_GrabbingController    = null;
            }
        }
        /// <summary>
        /// On frame (late)
        /// </summary>
        private void LateUpdate()
        {
            if (m_GrabbingController == null)
                return;

            var l_Delta = m_GrabbingController.Axis0Joystick.y * Time.unscaledDeltaTime;
            if (m_GrabPosition.magnitude > MinScrollDistance)   m_GrabPosition -= Vector3.forward * l_Delta;
            else                                                m_GrabPosition -= Vector3.forward * Mathf.Clamp(l_Delta, float.MinValue, 0f);

            var l_RealPosition = m_GrabbingController.RawTransform.TransformPoint(m_GrabPosition);
            var l_RealRotation = m_GrabbingController.RawTransform.rotation * m_GrabRotation;

            m_FloatingPanel.RTransform.position = Vector3.Lerp(m_FloatingPanel.RTransform.position,       l_RealPosition, 10f * Time.unscaledDeltaTime);
            m_FloatingPanel.RTransform.rotation = Quaternion.Slerp(m_FloatingPanel.RTransform.rotation,   l_RealRotation,  5f * Time.unscaledDeltaTime);
        }
    }
}
