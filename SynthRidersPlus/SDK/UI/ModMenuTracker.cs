using UnityEngine;

namespace SynthRidersPlus.SDK.UI
{
    /// <summary>
    /// Mod menu tracker
    /// </summary>
    internal class ModMenuTracker : CP_SDK.Unity.PersistentSingleton<ModMenuTracker>
    {
        private static GameObject m_ZWrap = null;
        private static GameObject m_Home = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On frame
        /// </summary>
        private void Update()
        {
            if (CP_SDK.ChatPlexSDK.ActiveGenericScene != CP_SDK.ChatPlexSDK.EGenericScene.Menu)
                return;

            if (!m_ZWrap)
                m_ZWrap = GameObject.Find("Z-Wrap");

            if (!m_Home && m_ZWrap)
                m_Home = m_ZWrap.transform.Find("Home")?.gameObject;

            var l_ModMenuActive     = CP_SDK.UI.ModMenu.Instance?.gameObject.activeSelf ?? false;
            var l_ShouldBeActive    = false;

            if (m_ZWrap.activeSelf && m_ZWrap.transform.localScale != Vector3.zero)
                l_ShouldBeActive = true;

            if (l_ShouldBeActive && m_Home.activeSelf)
                l_ShouldBeActive = true;
            else
                l_ShouldBeActive = false;

            if (l_ShouldBeActive && !l_ModMenuActive)
                CP_SDK.UI.ModMenu.Instance.Present();
            else if (!l_ShouldBeActive && l_ModMenuActive)
                CP_SDK.UI.ModMenu.Instance.Dismiss();
        }
    }
}
