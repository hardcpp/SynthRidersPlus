using UnityEngine;

namespace SynthRidersPlus.SDK.UI.DefaultComponentsOverrides
{
    /// <summary>
    /// SynthRiders CFloatingPanel component
    /// </summary>
    internal class SR_CFloatingPanel : CP_SDK.UI.DefaultComponents.DefaultCFloatingPanel
    {
        /// <summary>
        /// Mover handle
        /// </summary>
        private Subs.SubFloatingPanelMoverHandle m_MoverHandle;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On component creation
        /// </summary>
        public override void Init()
        {
            var l_ShouldContinue = !m_RTransform;
            base.Init();

            if (!l_ShouldContinue)
                return;

            CreateMover();
            SetAllowMovement(false);

            CP_SDK.ChatPlexSDK.OnGenericSceneChange += ChatPlexSDK_OnGenericSceneChange;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set allow movements
        /// </summary>
        /// <param name="p_Allow">Is allowed?</param>
        /// <returns></returns>
        public override CP_SDK.UI.Components.CFloatingPanel SetAllowMovement(bool p_Allow)
        {
            base.SetAllowMovement(p_Allow);

            if (m_MoverHandle)
                m_MoverHandle.gameObject.SetActive(p_Allow);

            if (p_Allow)
            {
                var l_XRInputSystem = CP_SDK.XRInput.XRInputSystem.Instance;
                if (l_XRInputSystem && !l_XRInputSystem.GetComponent<Subs.SubFloatingPanelMover>())
                    l_XRInputSystem.gameObject.AddComponent<Subs.SubFloatingPanelMover>();
            }

            return this;
        }
        /// <summary>
        /// Set size
        /// </summary>
        /// <param name="p_Size">New size</param>
        /// <returns></returns>
        public override CP_SDK.UI.Components.CFloatingPanel SetSize(Vector2 p_Size)
        {
            base.SetSize(p_Size);
            UpdateMover();
            return this;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On component destroy
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create mover
        /// </summary>
        private void CreateMover()
        {
            var l_XRInputSystem = CP_SDK.XRInput.XRInputSystem.Instance;
            if (!l_XRInputSystem)
                return;

            if (!l_XRInputSystem.GetComponent<Subs.SubFloatingPanelMover>())
                l_XRInputSystem.gameObject.AddComponent<Subs.SubFloatingPanelMover>();

            if (m_MoverHandle == null)
            {
                m_MoverHandle = new GameObject("MoverHandle", typeof(Subs.SubFloatingPanelMoverHandle)).GetComponent<Subs.SubFloatingPanelMoverHandle>();
                m_MoverHandle.transform.SetParent(transform);
                m_MoverHandle.transform.localPosition   = Vector3.zero;
                m_MoverHandle.transform.localRotation   = Quaternion.identity;
                m_MoverHandle.transform.localScale      = Vector3.one;
                m_MoverHandle.FloatingPanel             = this;

                UpdateMover();
            }
        }
        /// <summary>
        /// Update mover collision
        /// </summary>
        private void UpdateMover()
        {
            if (m_MoverHandle == null)
                return;

            var l_Size = m_RTransform.sizeDelta;

            m_MoverHandle.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
            m_MoverHandle.transform.localRotation = Quaternion.identity;
            m_MoverHandle.transform.localScale    = new Vector3(l_Size.x, l_Size.y, 0.1f);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On generic scene change
        /// </summary>
        /// <param name="p_GenericScene">New generic scene</param>
        private void ChatPlexSDK_OnGenericSceneChange(CP_SDK.ChatPlexSDK.EGenericScene p_GenericScene)
            => CreateMover();
    }
}
