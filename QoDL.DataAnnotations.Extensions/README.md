# QoDL.DataAnnotations.Extensions

[![Nuget](https://img.shields.io/nuget/v/QoDL.DataAnnotations.Extensions?label=HealthCheckQoDL.DataAnnotations.Extensions&logo=nuget)](https://www.nuget.org/packages/QoDL.DataAnnotations.Extensions)

## What is it

A few extensions for modelstate to reduce boilerplate code.

## Example

```csharp
[HttpPost]
public ActionResult Submit(TestModel model)
{
    if (!ModelState.IsValid)
    {
        // Create a result from modelstate, containing any errors.
        return ModelState.CreateJsonResult();
    }

    var result = _service.Update(model);
    if (!result.Success)
    {
        // Optionally pass along a generic error message.
        return ModelState.CreateJsonResult(result.Error);
    }

    // If modelstate is valid, this creates a successful result.
    return ModelState.CreateJsonResult();

    // Variations:
    // ModelState.CreateJsonResultSuccess()
    // ModelState.CreateJsonResultSuccess<TData>(data)
    // ModelState.CreateJsonResultWithError(error)
    // ModelState.CreateJsonResult()
    // ModelState.CreateJsonResult(data)
}
```

## Developer details

To enable developer details output you must configure `QoDLDataAnnotationsGlobalConfig.EnableDeveloperDetails` to return true.
