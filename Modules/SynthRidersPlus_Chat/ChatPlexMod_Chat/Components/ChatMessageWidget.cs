﻿using CP_SDK.Chat.Interfaces;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChatPlexMod_Chat.Components
{
    /// <summary>
    /// Enhanced text mesh pro UGUI with background support
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    internal class ChatMessageWidget : MonoBehaviour
    {
        internal static float s_TopDownMargins = 1;
        internal static float s_LeftRightMargins = 4;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private Vector3 m_LocalPosition = Vector3.zero;
        private Image   m_Highlight     = null;
        private Image   m_Accent        = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal RectTransform      RTranform       = null;
        internal IChatService       Service         = null;
        internal ChatMessageText    Text            = null;
        internal ChatMessageText    SubText         = null;
        internal bool               EnableCallback  = false;
        internal float              Height          = 0f;
        internal float              PositionY       = 0f;

        internal event Action OnLatePreRenderRebuildComplete;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Accent color
        /// </summary>
        internal Color AccentColor
        {
            get => m_Accent.color;
            set => m_Accent.color = value;
        }
        /// <summary>
        /// Highlight color
        /// </summary>
        internal Color HighlightColor
        {
            get => m_Highlight.color;
            set => m_Highlight.color = value;
        }
        /// <summary>
        /// Enable or disable Highlight
        /// </summary>
        internal bool HighlightEnabled
        {
            get => m_Highlight.enabled;
            set => m_Highlight.enabled = value;
        }
        /// <summary>
        /// Enable or disable Accent
        /// </summary>
        internal bool AccentEnabled
        {
            get => m_Accent.enabled;
            set => m_Accent.enabled = value;
        }
        /// <summary>
        /// Enable or disable SubText
        /// </summary>
        internal bool SubTextEnabled
        {
            get => SubText.enabled;
            set => SubText.enabled = value;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// When the GameObject and his components is ready
        /// </summary>
        private void Awake()
        {
            RTranform = transform as RectTransform;

            /// Prepare highlight
            m_Highlight = gameObject.AddComponent<Image>();
            m_Highlight.raycastTarget   = false;
            m_Highlight.material        = CP_SDK.UI.UISystem.Override_GetUIMaterial();

            RTranform.anchorMin  = new Vector2(0.5f, 0f);
            RTranform.anchorMax  = new Vector2(0.5f, 0f);
            RTranform.pivot      = new Vector2(0.5f, 0f);

            /// Prepare text
            Text = new GameObject().AddComponent<ChatMessageText>();
            Text.rectTransform.anchorMin            = new Vector2(0f, 1f);
            Text.rectTransform.anchorMax            = new Vector2(0f, 1f);
            Text.rectTransform.pivot                = new Vector2(0f, 1f);
            Text.rectTransform.localPosition        = Vector3.zero;
            Text.margin                             = new Vector4(s_LeftRightMargins, s_TopDownMargins, s_LeftRightMargins, s_TopDownMargins);
            Text.raycastTarget                      = false;
            Text.enableWordWrapping                 = true;
            Text.fontStyle                          = FontStyles.Normal;
            Text.overflowMode                       = TextOverflowModes.Overflow;
            Text.alignment                          = TextAlignmentOptions.TopLeft;
            Text.OnLatePreRenderRebuildComplete += Text_OnLatePreRenderRebuildComplete;

            /// Prepare sub text
            SubText = new GameObject().AddComponent<ChatMessageText>();
            SubText.rectTransform.anchorMin         = new Vector2(0f, 1f);
            SubText.rectTransform.anchorMax         = new Vector2(0f, 1f);
            SubText.rectTransform.pivot             = new Vector2(0f, 1f);
            SubText.rectTransform.localPosition     = Vector3.zero;
            SubText.margin                          = new Vector4(s_LeftRightMargins, s_TopDownMargins, s_LeftRightMargins, s_TopDownMargins);
            SubText.raycastTarget                   = false;
            SubText.enableWordWrapping              = true;
            SubText.fontStyle                       = FontStyles.Normal;
            SubText.overflowMode                    = TextOverflowModes.Overflow;
            SubText.alignment                       = TextAlignmentOptions.TopLeft;
            SubText.OnLatePreRenderRebuildComplete += Text_OnLatePreRenderRebuildComplete;

            /// Accent image
            m_Accent = new GameObject().AddComponent<Image>();
            m_Accent.raycastTarget  = false;
            m_Accent.type           = Image.Type.Sliced;
            m_Accent.material       = CP_SDK.UI.UISystem.Override_GetUIMaterial();
            m_Accent.sprite         = CP_SDK.UI.UISystem.GetUIRoundBGSprite();
            m_Accent.color          = Color.white;

            /// Disable all sub element by default
            SubTextEnabled      = false;
            HighlightEnabled    = false;
            AccentEnabled       = false;

            Text.rectTransform.SetParent(gameObject.transform, false);
            SubText.rectTransform.SetParent(gameObject.transform, false);
            m_Accent.gameObject.transform.SetParent(gameObject.transform, false);

            /// Update accent Layout UI transform
            (m_Accent.gameObject.transform as RectTransform).anchorMin  = new Vector2(0, 0.5f);
            (m_Accent.gameObject.transform as RectTransform).anchorMax  = new Vector2(0, 0.5f);
            (m_Accent.gameObject.transform as RectTransform).sizeDelta  = new Vector2(1, 10);
            (m_Accent.gameObject.transform as RectTransform).pivot      = new Vector2(0, 0.5f);
        }
        /// <summary>
        /// When the GameObject is destroyed
        /// </summary>
        private void OnDestroy()
        {
            Text.OnLatePreRenderRebuildComplete     -= Text_OnLatePreRenderRebuildComplete;
            SubText.OnLatePreRenderRebuildComplete  -= Text_OnLatePreRenderRebuildComplete;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set Y position
        /// </summary>
        /// <param name="p_PositionY">new Y position</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetPositionY(float p_PositionY)
        {
            m_LocalPosition.y           = p_PositionY;
            PositionY                   = p_PositionY;
            RTranform.localPosition  = m_LocalPosition;
        }
        /// <summary>
        /// Set width
        /// </summary>
        /// <param name="p_Width"></param>
        internal void SetWidth(float p_Width)
        {
            Text.rectTransform.sizeDelta    = new Vector2(p_Width, Text.rectTransform.sizeDelta.y);
            SubText.rectTransform.sizeDelta = new Vector2(p_Width, SubText.rectTransform.sizeDelta.y);
            RTranform.sizeDelta             = new Vector2(p_Width, RTranform.sizeDelta.y);

            OnTextChanged();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// When the text did render
        /// </summary>
        private void Text_OnLatePreRenderRebuildComplete()
        {
            OnTextChanged();

            if (EnableCallback)
                OnLatePreRenderRebuildComplete?.Invoke();
        }
        /// <summary>
        /// On text changed, update all width/height
        /// </summary>
        private void OnTextChanged()
        {
            float l_TextHeight      = Mathf.Max(0, Text.GetRenderedValues().y + (2 * s_TopDownMargins));
            float l_SubTextHeight   = SubTextEnabled ? Mathf.Max(0, SubText.GetRenderedValues().y + (2 * s_TopDownMargins)) : 0;

            //if (l_TextHeight == 0)
            //    l_TextHeight = Text.GetPreferredValues(" ").y;

            Height                  = l_TextHeight + l_SubTextHeight;
            RTranform.sizeDelta  = new Vector2(RTranform.sizeDelta.x, Height);

            if (SubText.enabled)
                SubText.rectTransform.localPosition = new Vector3(SubText.rectTransform.localPosition.x, l_SubTextHeight, SubText.rectTransform.localPosition.z);

            if (m_Accent.enabled)
                m_Accent.rectTransform.sizeDelta = new Vector2(s_LeftRightMargins / 2f, RTranform.sizeDelta.y);
        }
    }
}
