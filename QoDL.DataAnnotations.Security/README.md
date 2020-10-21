# QoDL.DataAnnotations.Security

[![Nuget](https://img.shields.io/nuget/v/QoDL.DataAnnotations.Security?label=QoDL.DataAnnotations.Security&logo=nuget)](https://www.nuget.org/packages/QoDL.DataAnnotations.Security)

## What is it

A few quick to implement security enhancements.

## GhostField

Namespace: `QoDL.DataAnnotations.Security.GhostField`

Use the `AddAntiSpamGhostField` helper method to insert a hidden input field for robots to fill out that causes an error when validated using the `ValidateGhostFieldAttribute`.

```csharp
@Html.AddAntiSpamGhostField()
```

```csharp
[HttpPost]
[ValidateGhostField]
public ActionResult Submit(SomeModel model)
{
    ...
}
```

By default the validation attribute returns a `HttpUnauthorizedResult` if any form value with the name `remarks` is submitted. This name can be changed in the extension method and attribute if needed.

## SessionTokenValidation

Namespace: `QoDL.DataAnnotations.Security.SessionToken`

Validates requests with a generated token, refreshing the page or returning an error when token fails to invalidate.

### Validation

Validate using the `ValidateSessionTokenAttribute`.

```csharp
[HttpPost]
[ValidateSessionToken]
public ActionResult Submit(SomeModel model)
{
    ...
}
```

By default the validation attribute refreshes the page if validation fails, allowing any form to be filled out again. After a successfull validation a new token i generated. The token is returned in the response header `___ValidateSessionToken`, and all valid tokens are set in the cookie also named `___ValidateSessionTokens`.

The first token found is validated in the following order: Form input, header value, cookie values. On a successfull validation the token is invalidated.

### Usage in views

Use the `AddValidateSessionTokenField` helper method to insert a hidden input field with a generated token that can only be validated once.

```csharp
@Html.AddValidateSessionTokenField()
```

When validation fails `___SessionTokenValidationFailed` will be set to `"1"` in TempData. Shortcuts to check for this can be found in `ValidateSessionTokenAttribute.DidValidationFailThisRequest(TempData)` or `Html.DidSessionTokenValidationFailThisRequest()` to show a suitable error message after auto-refreshing the page.

```csharp
@if (Html.DidSessionTokenValidationFailThisRequest())
{
    <div class="error">Something failed, please try submitting the form again.</div>
}
```

### Usage in API calls

Initial calls require a token that can be retrieved using `@Html.GetNewValidateSessionToken()` or `ValidateSessionTokenAttribute.CreateNewToken()`. Sequential calls will automatically succeed as long as cookies are enabled. Alternatively a new token can be retrieved from the response header `___ValidateSessionToken`.
