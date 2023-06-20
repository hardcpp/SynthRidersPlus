namespace ChatPlexMod_ChatIntegrations.UI
{
    /// <summary>
    /// Event list item
    /// </summary>
    internal class EventListItem : CP_SDK.UI.Data.IListItem
    {
        public Interfaces.IEventBase Event;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p_Event">Event</param>
        public EventListItem(Interfaces.IEventBase p_Event)
        {
            Event = p_Event;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On show
        /// </summary>
        public override void OnShow() => Refresh();
        /// <summary>
        /// On hide
        /// </summary>
        public override void OnHide() { }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Refresh
        /// </summary>
        public void Refresh()
        {
            if (Cell == null || !(Cell is CP_SDK.UI.Data.TextListCell l_TextListCell))
                return;

            var l_Text = "<line-height=1%><align=\"left\">";

            if (!Event.IsEnabled)
                l_Text += "<alpha=#70><s>";
            else
                l_Text += "<b>";

            l_Text += (Event.IsEnabled ? "<color=yellow>" : "") + "[" + Event.GetTypeName() + "] " + (Event.IsEnabled ? "</color>" : "");
            l_Text += " ";
            l_Text += Event.GenericModel.Name;

            if (!Event.IsEnabled)
                l_Text += "</s> (Disabled)";
            else
                l_Text += "</b>";

            l_Text += "\n<line-height=100%><align=\"right\">Used " + Event.GenericModel.UsageCount + " time(s)";

            l_TextListCell.Text.SetText(l_Text);
        }
    }
}
