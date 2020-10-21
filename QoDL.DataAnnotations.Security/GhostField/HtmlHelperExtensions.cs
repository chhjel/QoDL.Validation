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
        /// <param name="name">Optionally override input name.
        /// If overridden the same name must be provided in <see cref="ValidateGhostFieldAttribute"/>.</param>
        public static MvcHtmlString AddAntiSpamGhostField(this HtmlHelper html, string name = "remarks")
        {
            var style = "position: fixed; transform: translateX(100vw);";
            var content = $"<div class=\"remarks--wrapper\" style=\"{style}\"><label>Remarks</label><input class=\"remark--input\" name=\"{name}\" placeholder=\"yourname@yourdomain.com\" tabindex=\"-1\"></div>";
            return new MvcHtmlString(content);
        }
    }
}
#endif
