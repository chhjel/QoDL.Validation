#if NETFULL
using System.Web.Mvc;

namespace QoDL.DataAnnotations.Security.SessionToken
{
    /// <summary>
    /// Extension methods for <see cref="HtmlHelper"/>.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Inserts a hidden form input with a generated token.
        /// <para>Validate using <see cref="ValidateSessionTokenAttribute"/>.</para>
        /// </summary>
        public static MvcHtmlString AddValidateSessionTokenField(this HtmlHelper html)
        {
            var name = ValidateSessionTokenAttribute._formInputName;
            var token = ValidateSessionTokenAttribute.CreateNewToken();

            var content = $"<input name=\"{name}\" type=\"hidden\" value=\"{token}\" />";
            return new MvcHtmlString(content);
        }

        /// <summary>
        /// Retuns a generated token.
        /// <para>Validate using <see cref="ValidateSessionTokenAttribute"/>.</para>
        /// </summary>
        public static MvcHtmlString GetNewValidateSessionToken(this HtmlHelper html)
        {
            var token = ValidateSessionTokenAttribute.CreateNewToken();

            return new MvcHtmlString(token);
        }

        /// <summary>
        /// Check if <see cref="ValidateSessionTokenAttribute"/> failed this request.
        /// <para>For use with <see cref="InvalidSessionTokenResponse.RedirectBack"/> to show an error after the redirect.</para>
        /// <para>Shortcut to <see cref="ValidateSessionTokenAttribute.DidValidationFailThisRequest"/></para>
        /// </summary>
        public static bool DidSessionTokenValidationFailThisRequest(this HtmlHelper html)
            => ValidateSessionTokenAttribute.DidValidationFailThisRequest(html.ViewContext.TempData);
    }
}
#endif
