using Microsoft.AspNetCore.Components;

namespace PRUNStatsApp.Components.Base
{
    /// <summary>
    /// A container component that is styled to look like a Prosperous Universe window.
    /// </summary>
    public partial class PRContainer : ComponentBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public string Style { get; set; } = string.Empty;

        /// <summary>
        /// Defaults to "pa-2"
        /// </summary>
        [Parameter]
        public string InternalPadding { get; set; } = "pa-2";
    }
}
