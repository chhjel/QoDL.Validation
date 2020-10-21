#if NETFULL

namespace QoDL.DataAnnotations.Security.SessionToken
{
    /// <summary>
    /// How to handle invalid tokens.
    /// </summary>
    public enum InvalidSessionTokenResponse
    {
        /// <summary>
        /// Redirect where the request originated from without showing any errors, allowing any form to be refilled.
        /// <para><see cref="ValidateSessionTokenAttribute.DidValidationFailThisRequest"/> will return true for the rest of the request.</para>
        /// </summary>
        RedirectBack = 0,

        /// <summary>
        /// Return an unauthorized result.
        /// </summary>
        UnauthorizedResult
    }
}
#endif
