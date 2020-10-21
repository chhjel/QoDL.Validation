#if NETFULL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QoDL.DataAnnotations.Security.SessionToken
{
    /// <summary>
    /// Redirects or returns an UnauthorizedResult if token fails to validate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateSessionTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// Max number of keys to store per session.
        /// <para>Global config, defaults to 10.</para>
        /// </summary>
        protected static int MaxStoredKeysPerSession { get; set; } = 10;

        /// <summary>
        /// Url to redirect to on validation fail.
        /// <para>Defaults to previous url with <see cref="FallbackRedirectUrl"/> as fallback.</para>
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Max redirect count before fallback url is used, to prevent being stuck in a loop accidentally.
        /// <para>Defaults to 3.</para>
        /// </summary>
        public int MaxRedirectCount { get; set; } = 3;

        /// <summary>
        /// Fallback url to redirect to if stuck in a redirect loop or no suitable url was found to redirect to.
        /// <para>Defaults to /</para>
        /// </summary>
        public string FallbackRedirectUrl { get; set; } = "/";

        /// <summary>
        /// Enable sending and consuming tokens from cookies.
        /// <para>Defaults to true.</para>
        /// </summary>
        public bool EnableCookies { get; set; } = true;

        internal const string _formInputName = "___ValidateSessionToken";
        private const string _cookieName = "___ValidateSessionTokens";
        private const string _sessionKey = "___ValidateSessionToken";
        private const string _headerKey = "___ValidateSessionToken";
        private const string _httpContextItemsKeyTokenCache = "___ValidateSessionToken";
        private const string _httpContextItemsKeyValidationFailed = "___SessionTokenValidationFailed";
        private readonly InvalidSessionTokenResponse _invalidTokenResponse;

        /// <summary>
        /// Redirects or returns an UnauthorizedResult if token fails to validate.
        /// </summary>
        public ValidateSessionTokenAttribute(InvalidSessionTokenResponse invalidTokenResponse = InvalidSessionTokenResponse.RedirectBack)
        {
            _invalidTokenResponse = invalidTokenResponse;
        }

        /// <summary>
        /// Redirects or returns an UnauthorizedResult if token fails to validate.
        /// </summary>
        public ValidateSessionTokenAttribute(string redirectUrlOnInvalid)
            : this(InvalidSessionTokenResponse.RedirectBack)
        {
            RedirectUrl = redirectUrlOnInvalid;
        }

        /// <summary>
        /// Check if the token validation failed this request.
        /// </summary>
        public static bool DidValidationFailThisRequest(TempDataDictionary tempData)
            => tempData?[_httpContextItemsKeyValidationFailed] as string == "1";

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

            var session = HttpContext.Current.Request.RequestContext.HttpContext.Session;
            List<string> sessionTokens = EnsureSessionTokenList(session);

            var tokenFromFormOrHeader = request.Form[_formInputName]
                ?? request.Headers[_headerKey];
            var tokensFromCookie = EnableCookies
                ? request.Cookies[_cookieName]?.Value?.Split(':').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray()
                : new string[0];

            var tokensFromRequest = tokenFromFormOrHeader != null
                ? new[] { tokenFromFormOrHeader }
                : tokensFromCookie;

            if (tokensFromRequest?.Any() != true
                || !tokensFromRequest.Any(token => sessionTokens.Contains(token)))
            {
                OnTokenInvalidInternal(filterContext);
                return;
            }

            var tokenToExpire = tokensFromRequest.FirstOrDefault(x => sessionTokens.Contains(x));
            sessionTokens.Remove(tokenToExpire);

            CreateNewToken(EnableCookies);
        }

        private void OnTokenInvalidInternal(AuthorizationContext filterContext)
        {
            filterContext.Controller.TempData[_httpContextItemsKeyValidationFailed] = "1";
            OnTokenInvalid(filterContext);
        }

        /// <summary>
        /// Override this with custom logic if needed.
        /// </summary>
        protected virtual void OnTokenInvalid(AuthorizationContext filterContext)
        {
            if (_invalidTokenResponse == InvalidSessionTokenResponse.UnauthorizedResult)
            {
                filterContext.Result = new HttpUnauthorizedResult("Session validation token failed. Refresh the page and try again.");
            }
            else if (_invalidTokenResponse == InvalidSessionTokenResponse.RedirectBack)
            {
                var redirectCount = IncreaseRedirectCount(filterContext);
                if (redirectCount >= MaxRedirectCount)
                {
                    filterContext.Result = new RedirectResult(FallbackRedirectUrl);
                }

                var redirectToUrl = GetRedirectUrl(filterContext);
                filterContext.Result = new RedirectResult(redirectToUrl);
            }
        }

        /// <summary>
        /// Create a new token that is added to the session.
        /// </summary>
        public static string CreateNewToken(bool updateCookie = true)
        {
            // Check if we already created a token this request.
            var tokenCreatedThisRequest = HttpContext.Current.Items[_httpContextItemsKeyTokenCache] as string;
            if (tokenCreatedThisRequest != null)
            {
                return tokenCreatedThisRequest;
            }

            var newToken = Guid.NewGuid().ToString();
            var session = HttpContext.Current.Request.RequestContext.HttpContext.Session;
            List<string> sessionTokens = EnsureSessionTokenList(session);

            sessionTokens.Add(newToken);

            if (sessionTokens.Count > MaxStoredKeysPerSession)
            {
                for (int i = 0; i < (sessionTokens.Count - MaxStoredKeysPerSession); i++)
                {
                    sessionTokens.RemoveAt(0);
                }
            }

            // Send the new token in response
            var response = HttpContext.Current.Request.RequestContext.HttpContext.Response;
            response.Headers.Set(_headerKey, newToken);

            if (updateCookie)
            {
                response.Cookies.Set(new HttpCookie(_cookieName, string.Join(":", sessionTokens))
                {
                    HttpOnly = true
                });
            }

            HttpContext.Current.Items[_httpContextItemsKeyTokenCache] = newToken;
            return newToken;
        }

        /// <summary>
        /// Can be overridden to provide your own url to redirect to.
        /// </summary>
        protected virtual string GetRedirectUrl(AuthorizationContext filterContext)
        {
            var request = filterContext?.RequestContext?.HttpContext?.Request;
            return RedirectUrl ?? request?.UrlReferrer?.ToString() ?? FallbackRedirectUrl;
        }

        private static List<string> EnsureSessionTokenList(HttpSessionStateBase session)
        {
            if (!(session[_sessionKey] is List<string> sessionTokens))
            {
                session[_sessionKey] = new List<string>();
                sessionTokens = session[_sessionKey] as List<string>;
            }

            return sessionTokens;
        }

        private const string _redirectCountTempKey = "___ValidateSessionTokenRedirectCounter";

        private int GetRedirectCount(AuthorizationContext filterContext)
            => filterContext?.Controller?.TempData?[_redirectCountTempKey] as int? ?? 0;

        private int IncreaseRedirectCount(AuthorizationContext filterContext)
        {
            if (filterContext?.Controller?.TempData != null)
            {
                var newCount = GetRedirectCount(filterContext) + 1;
                filterContext.Controller.TempData[_redirectCountTempKey] = newCount;
                return newCount;
            }
            return 0;
        }
    }
}
#endif
