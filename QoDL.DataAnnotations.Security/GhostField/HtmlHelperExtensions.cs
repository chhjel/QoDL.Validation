#if NETFULL
using System.Web.Mvc;

namespace QoDL.DataAnnotations.Security.GhostField
{
    /// <summary>
    /// Extension methods for <see cref="HtmlHelper"/>.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Inserts a hidden form field for bots to fill out.
        /// <para>Validate using <see cref="ValidateGhostFieldAttribute"/>.</para>
        /// </summary>
        /// <param name="html"></param>
        /// <param name="name">Optionally override input name and wrapper class name.
        /// If overridden the same name must be provided in <see cref="ValidateGhostFieldAttribute"/>.</param>
        /// <param name="autoCompleteMode">Includes an autocomplete attribute.</param>
        public static MvcHtmlString AddAntiSpamGhostField(this HtmlHelper html, string name = "remarks",
            AutoCompleteMode autoCompleteMode = AutoCompleteMode.NewPassword)
        {
            var autoCompletePart = "";
            if (autoCompleteMode == AutoCompleteMode.Off)
            {
                autoCompletePart = " autocomplete=\"off\"";
            }
            else if(autoCompleteMode == AutoCompleteMode.NewPassword)
            {
                autoCompletePart = " autocomplete=\"new-password\"";
            }

            var style = "position: fixed; transform: translateX(100vw);";
            var content = $"<div class=\"{name}--wrapper\" style=\"{style}\"><label>Remarks</label><input class=\"remark--input\" name=\"{name}\" placeholder=\"yourname@yourdomain.com\" tabindex=\"-1\"{autoCompletePart}></div>";
            return new MvcHtmlString(content);
        }

        /// <summary>
        /// Autocomplete mode
        /// </summary>
        public enum AutoCompleteMode
        {
            /// <summary>
            /// Dont set any autocomplete attribute.
            /// </summary>
            Allow,

            /// <summary>
            /// Sets autocomplete="Off"
            /// </summary>
            Off,

            /// <summary>
            /// Sets autocomplete="new-password", might work better than "off".
            /// </summary>
            NewPassword
        }
    }
}
#endif
