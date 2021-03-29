﻿#if NETFULL
using QoDL.DataAnnotations.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace QoDL.DataAnnotations.Extensions.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="ModelStateDictionary"/>.
    /// </summary>
    public static class ModelStateDictionaryExtensions
    {
        /// <summary>
        /// Add an error for the given property by expression.
        /// </summary>
        public static ModelStateDictionary AddModelError<TModel>(this ModelStateDictionary modelState,
            Expression<Func<TModel, object>> expression, string message)
        {
            var targetName = expression.GetMemberName();

            modelState.AddModelError(targetName, message);
            return modelState;
        }

        /// <summary>
        /// Create a json result without any errors.
        /// </summary>
        public static ActionResult CreateJsonResultSuccess(this ModelStateDictionary modelState,
            IEnumerable<string> flags = null,
            JsonRequestBehavior requestBehavior = JsonRequestBehavior.AllowGet,
            int? statusCode = null,
            string devDetails = null)
        {
            var resultObject = new ModelValidatedResult
            {
                Success = true,
                Flags = flags.CreateHashSet()
            };
            QoDLDataAnnotationsGlobalConfig.ApplyDeveloperDetailsIfEnabled(resultObject, devDetails);
            return new JsonContentResult(resultObject, requestBehavior, statusCode);
        }

        /// <summary>
        /// Create a json result without any errors.
        /// </summary>
        public static ActionResult CreateJsonResultSuccess<TData>(this ModelStateDictionary modelState,
            TData data,
            IEnumerable<string> flags = null,
            JsonRequestBehavior requestBehavior = JsonRequestBehavior.AllowGet,
            int? statusCode = null,
            string devDetails = null)
        {
            var resultObject = new ModelValidatedResult<TData>
            {
                Success = true,
                Data = data,
                Flags = flags.CreateHashSet()
            };
            QoDLDataAnnotationsGlobalConfig.ApplyDeveloperDetailsIfEnabled(resultObject, devDetails);
            return new JsonContentResult(resultObject, requestBehavior, statusCode);
        }

        /// <summary>
        /// Create a json result with a dictionary containing any invalid property names and their errors.
        /// </summary>
        public static ActionResult CreateJsonResultWithError(this ModelStateDictionary modelState,
            string error,
            IEnumerable<string> flags = null,
            JsonRequestBehavior requestBehavior = JsonRequestBehavior.AllowGet,
            int? statusCode = null,
            string devDetails = null)
        {
            var modelErrors = modelState.GetModelErrorsAsDictionary();

            var resultObject = new ModelValidatedResult
            {
                Success = modelState.IsValid && string.IsNullOrWhiteSpace(error),
                Error = error,
                ModelErrors = modelErrors,
                Flags = flags.CreateHashSet()
            };
            QoDLDataAnnotationsGlobalConfig.ApplyDeveloperDetailsIfEnabled(resultObject, devDetails);
            return new JsonContentResult(resultObject, requestBehavior, statusCode);
        }

        /// <summary>
        /// Create a json result with a dictionary containing any invalid property names and their errors.
        /// </summary>
        public static ActionResult CreateJsonResultWithError<TData>(this ModelStateDictionary modelState,
            string error,
            TData data = null,
            IEnumerable<string> flags = null,
            JsonRequestBehavior requestBehavior = JsonRequestBehavior.AllowGet,
            int? statusCode = null,
            string devDetails = null)
            where TData: class
        {
            var modelErrors = modelState.GetModelErrorsAsDictionary();

            var resultObject = new ModelValidatedResult<TData>
            {
                Success = modelState.IsValid && string.IsNullOrWhiteSpace(error),
                Error = error,
                Data = data,
                ModelErrors = modelErrors,
                Flags = flags.CreateHashSet()
            };
            QoDLDataAnnotationsGlobalConfig.ApplyDeveloperDetailsIfEnabled(resultObject, devDetails);
            return new JsonContentResult(resultObject, requestBehavior, statusCode);
        }

        /// <summary>
        /// Create a json result with a dictionary containing any invalid property names and their errors.
        /// <para>Optionally with an error message if any model state issues were found.</para>
        /// </summary>
        public static ActionResult CreateJsonResult(this ModelStateDictionary modelState,
            IEnumerable<string> flags = null,
            JsonRequestBehavior requestBehavior = JsonRequestBehavior.AllowGet,
            int? statusCode = null,
            string errorMessageIfNotSuccess = null,
            string devDetails = null)
        {
            var modelErrors = modelState.GetModelErrorsAsDictionary();

            var success = modelState.IsValid;
            var resultObject = new ModelValidatedResult
            {
                Success = success,
                Error = success ? null : errorMessageIfNotSuccess,
                ModelErrors = modelErrors,
                Flags = flags.CreateHashSet()
            };
            QoDLDataAnnotationsGlobalConfig.ApplyDeveloperDetailsIfEnabled(resultObject, devDetails);
            return new JsonContentResult(resultObject, requestBehavior, statusCode);
        }

        /// <summary>
        /// Create a json result with a dictionary containing any invalid property names and their errors.
        /// </summary>
        public static ActionResult CreateJsonResult<TData>(this ModelStateDictionary modelState,
            TData data,
            IEnumerable<string> flags = null,
            string error = null,
            JsonRequestBehavior requestBehavior = JsonRequestBehavior.AllowGet,
            int? statusCode = null,
            string devDetails = null)
        {
            var modelErrors = modelState.GetModelErrorsAsDictionary();

            var resultObject = new ModelValidatedResult<TData>
            {
                Success = modelState.IsValid && string.IsNullOrWhiteSpace(error),
                Data = data,
                Flags = flags.CreateHashSet(),
                Error = error,
                ModelErrors = modelErrors
            };
            QoDLDataAnnotationsGlobalConfig.ApplyDeveloperDetailsIfEnabled(resultObject, devDetails);
            return new JsonContentResult(resultObject, requestBehavior, statusCode);
        }

        /// <summary>
        /// Get a dictionary of all property names and errors in the model.
        /// </summary>
        public static Dictionary<string, string> GetModelErrorsAsDictionary(this ModelStateDictionary modelState)
        {
            var modelErrors = new Dictionary<string, string>();

            if (!modelState.IsValid)
            {
                foreach (var err in modelState.Where(x => x.Value.Errors.Any()))
                {
                    var validationError = string.Join(". ", err.Value.Errors.Select(e => e.ErrorMessage));
                    modelErrors[err.Key] = validationError;
                }
            }

            return modelErrors;
        }
    }
}
#endif
