using UnityEngine;

namespace SynthRidersPlus.SDK.UI
{
    /// <summary>
    /// Canvas blocker script
    /// </summary>
    internal class CanvasBlocker : MonoBehaviour
    {
        private CP_SDK.UI.Components.CFloatingPanel m_FloatingPanel;
        private GameObject                          m_Child;
        private BoxCollider                         m_BoxCollider;
        private Rigidbody                           m_Rigidbody;
        private VRTK.VRTK_InteractableObject        m_InteractableObject;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On component creation
        /// </summary>
        private void Awake()
        {
            m_FloatingPanel = GetComponent<CP_SDK.UI.Components.CFloatingPanel>();
            m_FloatingPanel.OnSizeChanged(OnSizeChanged);

            m_Child = new GameObject("CanvasBlockerChild");
            m_Child.transform.SetParent(transform);
            m_Child.transform.localPosition = Vector3.zero;
            m_Child.transform.localRotation = Quaternion.identity;
            m_Child.transform.localScale    = Vector3.one;
            m_Child.layer                   = 29;

            m_BoxCollider = m_Child.AddComponent<BoxCollider>();
            m_BoxCollider.size      = new Vector3(1f, 1f, 0.1f);

            m_Rigidbody = m_Child.AddComponent<Rigidbody>();
            m_Rigidbody.collisionDetectionMode  = CollisionDetectionMode.ContinuousSpeculative;
            m_Rigidbody.detectCollisions        = true;
            m_Rigidbody.isKinematic             = true;

            m_InteractableObject = m_Child.AddComponent<VRTK.VRTK_InteractableObject>();
            m_InteractableObject.isKinematic        = true;
            m_InteractableObject.disableWhenIdle    = true;

            /// Apply
            OnSizeChanged(m_FloatingPanel, m_FloatingPanel.RTransform.sizeDelta);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On floating panel size changed
        /// </summary>
        /// <param name="p_FloatingPanel">Floating panel instance</param>
        /// <param name="p_Size">New size</param>
        private void OnSizeChanged(CP_SDK.UI.Components.CFloatingPanel p_FloatingPanel, Vector2 p_Size)
        {
            m_Child.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
            m_Child.transform.localRotation = Quaternion.identity;
            m_Child.transform.localScale    = new Vector3(p_Size.x, p_Size.y, 0.1f);
        }
    }
}
