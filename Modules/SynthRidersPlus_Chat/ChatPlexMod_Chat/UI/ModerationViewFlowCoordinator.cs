﻿using UnityEngine;

namespace ChatPlexMod_Chat.UI
{
    /// <summary>
    /// Moderation UI flow coordinator
    /// </summary>
    internal sealed class ModerationViewFlowCoordinator : CP_SDK.UI.FlowCoordinator<ModerationViewFlowCoordinator>
    {
        public override string Title => "Chat Moderation";

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private ModerationLeftView          m_LeftView;
        private ModerationMainView          m_MainView;
        private ModerationRightView         m_RightView;
        private ModerationShortcutsMainView m_ShortcutsMainView;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Constructor
        /// </summary>
        public override void Init()
        {
            m_LeftView          = CP_SDK.UI.UISystem.CreateViewController<ModerationLeftView>();
            m_MainView          = CP_SDK.UI.UISystem.CreateViewController<ModerationMainView>();
            m_RightView         = CP_SDK.UI.UISystem.CreateViewController<ModerationRightView>();
            m_ShortcutsMainView = CP_SDK.UI.UISystem.CreateViewController<ModerationShortcutsMainView>();
        }
        /// <summary>
        /// On destroy
        /// </summary>
        internal void OnDestroy()
        {
            if (m_MainView          != null) GameObject.Destroy(m_MainView.gameObject);
            if (m_LeftView          != null) GameObject.Destroy(m_LeftView.gameObject);
            if (m_RightView         != null) GameObject.Destroy(m_RightView.gameObject);
            if (m_ShortcutsMainView != null) GameObject.Destroy(m_ShortcutsMainView.gameObject);

            m_LeftView              = null;
            m_MainView              = null;
            m_RightView             = null;
            m_ShortcutsMainView     = null;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get initial views controller
        /// </summary>
        /// <returns>(Middle, Left, Right)</returns>
        protected override sealed (CP_SDK.UI.IViewController, CP_SDK.UI.IViewController, CP_SDK.UI.IViewController) GetInitialViewsController()
            => (m_MainView, m_LeftView, m_RightView);

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Switch to shortcut view
        /// </summary>
        internal void SwitchToShortcuts()
            => ChangeViewControllers(m_ShortcutsMainView);

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On back button pressed
        /// </summary>
        /// <param name="p_MainViewController">Current main view controller</param>
        /// <returns>True if the event is catched, false if we should dismiss the flow coordinator</returns>
        public override sealed bool OnBackButtonPressed(CP_SDK.UI.IViewController p_MainViewController)
        {
            if (p_MainViewController == m_ShortcutsMainView)
            {
                ChangeViewControllers(m_MainView, m_LeftView, m_RightView);
                return true;
            }

            return false;
        }
    }
}
