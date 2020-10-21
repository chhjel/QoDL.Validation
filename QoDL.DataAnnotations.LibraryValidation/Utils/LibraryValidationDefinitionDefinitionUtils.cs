using QoDL.DataAnnotations.LibraryValidation.Attributes;
using QoDL.DataAnnotations.LibraryValidation.Models;
using System;
using System.Linq;
using System.Reflection;

namespace QoDL.DataAnnotations.LibraryValidation.Utils
{
    /// <summary>
    /// Utils related to definition of library validation on models.
    /// </summary>
    public static class LibraryValidationDefinitionDefinitionUtils
    {
        /// <summary>
        /// Get data about what validations that are applied to properties on the given model type.
        /// </summary>
        public static ModelLibraryValidationDefinition GetModelLibraryValidationDefinition<TModel>()
            => GetModelLibraryValidationDefinition(typeof(TModel));

        /// <summary>
        /// Get data about what validations that are applied to properties on the given model type.
        /// </summary>
        public static ModelLibraryValidationDefinition GetModelLibraryValidationDefinition(Type modelType)
        {
            var definition = new ModelLibraryValidationDefinition();

            var members = modelType.GetMembers()
                .Where(x => x is FieldInfo || x is PropertyInfo)
                .Select(x => new
                {
                    Name = (x is FieldInfo field) ? field.Name : (x as PropertyInfo).Name,
                    Attributes = x.GetCustomAttributes()
                        .Where(x => x is LibraryValidationBaseAttribute)
                        .Cast<LibraryValidationBaseAttribute>()
                })
                .Where(x => x.Attributes.Any());

            foreach (var member in members)
            {
                var validatorData = member.Attributes
                    .Select(x => new
                    {
                        Type = x.ValidationTypeEnum,
                        Optional = x.Optional
                    })
                    .Where(x => x.Type != null)
                    .GroupBy(x => x.Type.ToString()).Select(x => x.First());

                definition[member.Name] = validatorData
                    .Select(x => new PropertyLibraryValidationDefinition
                    {
                        Optional = x.Optional,
                        Type = x.Type
                    })
                    .ToList();
            }

            return definition;
        }
    }
}
