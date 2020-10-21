# QoDL.DataAnnotations.LibraryValidation

[![Nuget](https://img.shields.io/nuget/v/QoDL.DataAnnotations.LibraryValidation?label=HealthCheckQoDL.DataAnnotations.LibraryValidation&logo=nuget)](https://www.nuget.org/packages/QoDL.DataAnnotations.LibraryValidation)

## What is it

A custom validation attribute that proxies the validation logic to a static library of validation functions targeted using your own enum. Using the bundled extension methods you can pass along a hint to frontend of what validation methods to run per property, making it a bit easier to keep validation on both ends in sync.

## Validation library setup

1. Create your own enum that contain available validation functions, e.g:

    ```csharp
    public enum LibraryValidationMethod
    {
        Required = 0,
        PhoneNumber,
        NotMobileNumber,
        Address,
        ZipCode,
        ...
    }
    ```

2. Create a static class that will contain validation methods, and decorate any methods with the `LibraryValidationMethod`-attribute, passing in an enum value with the validation the method will perform. See comments in the code below for more details about the methods. All methods with compatible input type and enum value will be executed.

    ```csharp
    public static class ValidationLibrary
    {
        // Required attribute
        [LibraryValidationMethod(LibraryValidationMethod.PhoneNumber)]
        public static string ValidatePhoneNumber(string input)
        {
            var error = "Invalid phone number";

            if (input == null || !int.TryParse(input, out int number))
            {
                // Return a string with the error if any.
                return error;
            }
            else if (number.ToString().Length != 8)
            {
                return error;
            }

            // Return null if there's no error
            return null;
        }

        // Methods can take 3 different parameters in any order.
        // - User input value.
        // - Type of the input value.
        // - ValidationContext object.
        [LibraryValidationMethod(LibraryValidationMethod.Required)]
        public static string ValidateRequired(object input, Type inputType, ValidationContext validationContext)
        {
            var error = string.Format(Dictionary.Validation.GenericRequired, validationContext.DisplayName);

            if (inputType == typeof(DateTime) && input == default)
            {
                return error;
            }
            else if (string.IsNullOrWhiteSpace(input?.ToString()))
            {
                return error;
            }

            return null;
        }
    }
    ```

3. Create an attribute inheriting `LibraryValidationBaseAttribute`, passing along your enum type and the class created in step #2.

    ```csharp
    public class LibraryValidationAttribute : LibraryValidationBaseAttribute
    {
        public LibraryValidationAttribute(LibraryValidationMethod method, bool optional = false)
            : base(typeof(ValidationLibrary), method, optional) { }
    }
    ```

4. Decorate models with your new attribute.

    ```csharp
    public class TestModel
    {
        [LibraryValidation(LibraryValidationMethod.Required)]
        [LibraryValidation(LibraryValidationMethod.PhoneNumber)]
        [LibraryValidation(LibraryValidationMethod.NotMobileNumber)]
        public string HomePhoneNumber { get; set; }

        [LibraryValidation(LibraryValidationMethod.Required)]
        [LibraryValidation(LibraryValidationMethod.PhoneNumber)]
        public int WorkPhoneNumber { get; set; }

        [LibraryValidation(LibraryValidationMethod.Required)]
        [LibraryValidation(LibraryValidationMethod.Address)]
        public string Address { get; set; }
    }
    ```

5. Validate modelstate as normal and optionally use any of the extension methods below to pass validation details to frontend.

### LibraryValidationBaseAttribute note

* If the optional-parameter passed to the constructor is true, any null input-values will not be sent to validator functions.

## Extension methods

Namespace containing all extension methods: `QoDL.DataAnnotations.LibraryValidation.Extensions`

### @Html.GetModelLibraryValidationDefinition()

Or `GetModelLibraryValidationDefinitionAsJson` to get the data as json.

Get a dictionary object containing the enum names and optional values from all attributes derived from `LibraryValidationBaseAttribute` applied to properties in the model.

The output can be used to map enum values to matching validation methods to execute per property in frontend.

Example output:

```json
{
  "homePhoneNumber": [
    { "type": "Required", "optional": false },
    { "type": "PhoneNumber", "optional": false },
    { "type": "NotMobileNumber", "optional": false }
  ],
  "workPhoneNumber": [
      { "type": "Required", "optional": false },
      { "type": "PhoneNumber", "optional": false }
  ],
  "address": [{ "type": "Address", "optional": true }]
}
```

### @Html.GetModelErrorsDictionary()

Or `GetModelErrorsDictionaryAsJson` to get the data as json.

Get a javascript compatible dictionary object containing all current validation errors along with their properties.

Example output:

```json
{
  "homePhoneNumber": "Number must be a landline number.",
  "workPhoneNumber": "Invalid phone number, it must be 8 digits."
}
```

### ModelState.CreateJsonResult()

Create a json action result containing a summary of the model validation along with an optional general error. Response statuscode can be customized.
The serialized type returned is either `ModelValidatedResult<TData>` or `ModelValidatedResult` depending on the overload used.

Example code:

```csharp
[HttpPost]
public ActionResult Submit(TestModel model)
{
    if (!ModelState.IsValid)
    {
        return ModelState.CreateJsonResult();
    }

    var result = _service.Update(model);
    if (result.Success)
    {
        // Optionally pass along a generic error message.
        return ModelState.CreateJsonResult(result.Error);
    }

    return ModelState.CreateJsonResult();

    // Variations:
    // ModelState.CreateJsonResultSuccess()
    // ModelState.CreateJsonResultSuccess<TData>(data)
    // ModelState.CreateJsonResultWithError(error)
    // ModelState.CreateJsonResult()
    // ModelState.CreateJsonResult(data)
}
```

Example output:

```json
{
 "success": false,
 "error": "",
 "modelErrors": {
    "homePhoneNumber": "Number must be a landline number.",
    "workPhoneNumber": "Invalid phone number, it must be 8 digits."
  },
  "flags": []
}
```
