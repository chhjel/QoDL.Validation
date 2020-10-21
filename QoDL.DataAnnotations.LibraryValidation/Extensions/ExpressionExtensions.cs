using System;
using System.Linq.Expressions;
using System.Reflection;

namespace QoDL.DataAnnotations.LibraryValidation.Extensions
{
    /// <summary>
    /// Extensions for <see cref="Expression"/>s.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Get the name of the field/property an expression points to.
        /// </summary>
        public static string GetMemberName<TModel, TProperty>(this Expression<Func<TModel, TProperty>> propertyLambda)
        {
            if (!(propertyLambda.Body is MemberExpression member))
            {
                throw new ArgumentException($"Expression '{propertyLambda}' ({propertyLambda?.Body?.GetType().Name}) refers to a method, not a property/field.");
            }

            var propInfo = member.Member as PropertyInfo;
            var memberName = propInfo?.Name;

            if (memberName == null)
            {
                var fieldInfo = member.Member as FieldInfo;
                memberName = fieldInfo?.Name;
            }

            return memberName;
        }
    }
}
