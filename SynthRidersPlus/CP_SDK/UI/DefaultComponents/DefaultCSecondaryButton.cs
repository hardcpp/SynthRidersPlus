﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace CP_SDK.UI.DefaultComponents
{
    /// <summary>
    /// Default CSecondaryButton component
    /// </summary>
    public class DefaultCSecondaryButton : Components.CSecondaryButton, IPointerEnterHandler, IPointerExitHandler
    {
        private RectTransform               m_RTransform;
        private ContentSizeFitter           m_CSizeFitter;
        private LayoutElement               m_LElement;
        private Button                      m_Button;
        private Image                       m_BackgroundImage;
        private Image                       m_IconImage;
        private Components.CText            m_Label;
        private Subs.SubStackLayoutGroup    m_StackLayoutGroup;
        private string                      m_Tooltip;
        private event Action                m_OnClick;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public override RectTransform       RTransform          => m_RTransform;
        public override ContentSizeFitter   CSizeFitter         => m_CSizeFitter;
        public override LayoutElement       LElement            => m_LElement;
        public override Button              ButtonC             => m_Button;
        public override Image               BackgroundImageC    => m_BackgroundImage;
        public override Image               IconImageC          => m_IconImage;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On component creation
        /// </summary>
        public virtual void Init()
        {
            if (m_RTransform)
                return;

            gameObject.layer = UISystem.UILayer;

            m_RTransform = transform as RectTransform;

            m_CSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            m_CSizeFitter.horizontalFit   = ContentSizeFitter.FitMode.PreferredSize;
            m_CSizeFitter.verticalFit     = ContentSizeFitter.FitMode.PreferredSize;

            m_LElement = gameObject.AddComponent<LayoutElement>();
            m_LElement.minHeight = 5f;

            m_BackgroundImage = new GameObject("BG", UISystem.Override_UnityComponent_Image).GetComponent(UISystem.Override_UnityComponent_Image) as Image;
            m_BackgroundImage.gameObject.layer = UISystem.UILayer;
            m_BackgroundImage.rectTransform.SetParent(transform, false);
            m_BackgroundImage.rectTransform.anchorMin     = Vector2.zero;
            m_BackgroundImage.rectTransform.anchorMax     = Vector2.one;
            m_BackgroundImage.rectTransform.sizeDelta     = Vector2.zero;
            m_BackgroundImage.rectTransform.localPosition = Vector2.zero;
            m_BackgroundImage.material                = UISystem.Override_GetUIMaterial();
            m_BackgroundImage.color                   = new Color32(255, 191, 37, 255);
            m_BackgroundImage.type                    = Image.Type.Sliced;
            m_BackgroundImage.pixelsPerUnitMultiplier = 1;
            m_BackgroundImage.sprite                  = UISystem.GetUIButtonSprite();

            m_Label = UISystem.TextFactory.Create("Label", transform);
            m_Label.SetMargins(2f, 0f, 2f, 0f);
            m_Label.SetAlign(TMPro.TextAlignmentOptions.Capline);

            m_IconImage = new GameObject("Icon", UISystem.Override_UnityComponent_Image).GetComponent(UISystem.Override_UnityComponent_Image) as Image;
            m_IconImage.gameObject.layer = UISystem.UILayer;
            m_IconImage.rectTransform.SetParent(transform, false);
            m_IconImage.rectTransform.anchorMin     = Vector2.zero;
            m_IconImage.rectTransform.anchorMax     = Vector2.one;
            m_IconImage.rectTransform.sizeDelta     = Vector2.zero;
            m_IconImage.rectTransform.localPosition = Vector2.zero;
            m_IconImage.material                = UISystem.Override_GetUIMaterial();
            m_IconImage.type                    = Image.Type.Simple;
            m_IconImage.pixelsPerUnitMultiplier = 1;
            m_IconImage.preserveAspect          = true;
            m_IconImage.gameObject.SetActive(false);

            m_Button = gameObject.AddComponent<Button>();
            m_Button.targetGraphic  = m_BackgroundImage;
            m_Button.transition     = Selectable.Transition.ColorTint;
            m_Button.onClick.RemoveAllListeners();
            m_Button.onClick.AddListener(Button_OnClick);

            var l_Colors = m_Button.colors;
            l_Colors.normalColor        = new Color32(255, 255, 255, 180);
            l_Colors.highlightedColor   = new Color32(255, 255, 255, 255);
            l_Colors.pressedColor       = new Color32(200, 200, 200, 255);
            l_Colors.selectedColor      = l_Colors.normalColor;
            l_Colors.disabledColor      = new Color32(255, 255, 255,  68);
            l_Colors.fadeDuration       = 0.05f;
            m_Button.colors = l_Colors;

            m_StackLayoutGroup = gameObject.AddComponent<Subs.SubStackLayoutGroup>();
            m_StackLayoutGroup.ChildForceExpandWidth    = true;
            m_StackLayoutGroup.ChildForceExpandHeight   = true;
            m_StackLayoutGroup.childAlignment           = TextAnchor.MiddleCenter;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On click event
        /// </summary>
        /// <param name="p_Functor">Functor to add/remove</param>
        /// <param name="p_Add">Should add</param>
        /// <returns></returns>
        public override Components.CPOrSButton OnClick(Action p_Functor, bool p_Add = true)
        {
            if (p_Add)  m_OnClick += p_Functor;
            else        m_OnClick -= p_Functor;

            return this;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get text
        /// </summary>
        /// <returns></returns>
        public override string GetText()
            => m_Label.GetText();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set font size
        /// </summary>
        /// <param name="p_Size">New size</param>
        /// <returns></returns>
        public override Components.CPOrSButton SetFontSize(float p_Size)
        {
            m_Label.SetFontSize(p_Size);
            return this;
        }
        /// <summary>
        /// Set overflow mode
        /// </summary>
        /// <param name="p_OverflowMode">New overflow mdoe</param>
        /// <returns></returns>
        public override Components.CPOrSButton SetOverflowMode(TextOverflowModes p_OverflowMode)
        {
            m_Label.SetOverflowMode(p_OverflowMode);
            return this;
        }
        /// <summary>
        /// Set button text
        /// </summary>
        /// <param name="p_Text">New text</param>
        /// <returns></returns>
        public override Components.CPOrSButton SetText(string p_Text)
        {
            m_Label.SetText(p_Text);
            return this;
        }
        /// <summary>
        /// Set tooltip
        /// </summary>
        /// <param name="p_Tooltip">New tooltip</param>
        /// <returns></returns>
        public override Components.CPOrSButton SetTooltip(string p_Tooltip)
        {
            m_Tooltip = p_Tooltip;
            return this;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On click unity callback
        /// </summary>
        private void Button_OnClick()
        {
            try { m_OnClick?.Invoke(); }
            catch (System.Exception l_Exception)
            {
                ChatPlexSDK.Logger.Error($"[CP_SDK.UI.DefaultComponents][DefaultCSecondaryButton.Button_OnClick] Error:");
                ChatPlexSDK.Logger.Error(l_Exception);
            }

            UISystem.Override_OnClick?.Invoke(this);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On pointer enter
        /// </summary>
        /// <param name="p_EventData"></param>
        public void OnPointerEnter(PointerEventData p_EventData)
        {
            if (string.IsNullOrEmpty(m_Tooltip))
                return;

            var l_ViewController = GetComponentInParent<IViewController>();
            if (!l_ViewController)
                return;

            var l_Rect = RTransform.rect;
            var l_RPos = new Vector2(l_Rect.x + l_Rect.width / 2f, l_Rect.y + l_Rect.height);
            var l_Pos  = RTransform.TransformPoint(l_RPos);
            l_ViewController.ShowTooltip(l_Pos, m_Tooltip);
        }
        /// <summary>
        /// On pointer exit
        /// </summary>
        /// <param name="p_EventData"></param>
        public void OnPointerExit(PointerEventData p_EventData)
        {
            var l_ViewController = GetComponentInParent<IViewController>();
            if (!l_ViewController)
                return;

            l_ViewController.HideTooltip();
        }
    }
}
