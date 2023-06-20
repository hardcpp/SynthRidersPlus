using UnityEngine;
using UnityEngine.UI;

namespace CP_SDK.UI.DefaultComponents
{
    /// <summary>
    /// Default CImage component
    /// </summary>
    public class DefaultCImage : Components.CImage
    {
        private RectTransform                   m_RTransform;
        private LayoutElement                   m_LElement;
        private Image                           m_Image;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public override RectTransform   RTransform  => m_RTransform;
        public override LayoutElement   LElement    => m_LElement;
        public override Image           ImageC      => m_Image;

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
            m_RTransform.sizeDelta = new Vector2(5f, 5f);

            m_LElement = gameObject.AddComponent<LayoutElement>();

            m_Image = gameObject.AddComponent(UISystem.Override_UnityComponent_Image) as Image;
            m_Image.material                = UISystem.Override_GetUIMaterial();
            m_Image.type                    = Image.Type.Simple;
            m_Image.pixelsPerUnitMultiplier = 1;
            m_Image.sprite                  = UISystem.GetUIRectBGSprite();
            m_Image.preserveAspect          = true;
            m_Image.raycastTarget           = false;
        }
    }
}
