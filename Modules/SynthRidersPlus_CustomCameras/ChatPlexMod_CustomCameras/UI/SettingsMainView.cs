using CP_SDK.XUI;

namespace ChatPlexMod_CustomCameras.UI
{
    /// <summary>
    /// CustomEdition settings view controller
    /// </summary>
    internal sealed class SettingsMainView : CP_SDK.UI.ViewController<SettingsMainView>
    {
        /// <summary>
        /// On view creation
        /// </summary>
        protected override sealed void OnViewCreation()
        {
            Templates.FullRectLayoutMainView(
                Templates.TitleBar("Custom Cameras | Settings"),

                XUIText.Make("No settings yet...")
            )
            .SetBackground(true)
            .BuildUI(transform);
        }
        /// <summary>
        /// On view deactivation
        /// </summary>
        protected override sealed void OnViewDeactivation()
        {
            CCConfig.Instance.Save();
        }
    }
}
