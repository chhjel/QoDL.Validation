#if NETFULL
using System;
using System.Web.Mvc;

namespace QoDL.DataAnnotations.Security.GhostField
{
    /// <summary>
    /// Validate ghost form field inserted using <see cref="HtmlHelperExtensions.AddAntiSpamGhostField"/>.
    /// <para>By default a <see cref="HttpUnauthorizedResult"/> is returned if the input contains data, this can be modified by creating your own derived version of this attribute.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateGhostFieldAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly string _formInputName;
        private readonly Action<AuthorizationContext> _customActionOnInvalid;

        /// <summary>
        /// Validate ghost form field inserted using <see cref="HtmlHelperExtensions.AddAntiSpamGhostField"/>.
        /// If a custom input name was given in <see cref="HtmlHelperExtensions.AddAntiSpamGhostField"/>, the same name must be given here.
        /// <para>By default a <see cref="HttpUnauthorizedResult"/> is returned if the input contains data, this can be modified by creating your own derived version of this attribute.</para>
        /// </summary>
        public ValidateGhostFieldAttribute(string inputName = "remarks")
            : this(inputName, null) { }

        /// <summary>
        /// Validate ghost form field inserted using <see cref="HtmlHelperExtensions.AddAntiSpamGhostField"/>.
        /// If a custom input name was given in <see cref="HtmlHelperExtensions.AddAntiSpamGhostField"/>, the same name must be given here.
        /// <para>This overload allows for a custom action when the input contains value.</para>
        /// </summary>
        protected ValidateGhostFieldAttribute(string inputName, Action<AuthorizationContext> customActionOnInvalid)
        {
            _formInputName = inputName;
            _customActionOnInvalid = customActionOnInvalid;
        }

        /// <summary>
        /// Performs validation.
        /// </summary>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            var request = filterContext.HttpContext?.Request;
            if (request == null)
            {
                return;
            }

            var value = filterContext.HttpContext.Request.Form[_formInputName];
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (_customActionOnInvalid != null)
                {
                    _customActionOnInvalid(filterContext);
                }
                else
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }
        }
    }
}
#endif
