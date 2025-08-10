# WeChooz Aspire Integration MailKit

[![CI](https://github.com/wechooz/aspire-integration-mailkit/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/wechooz/aspire-integration-mailkit/actions/workflows/ci.yml)

This .NET Aspire Integration registers [MailKit](https://mimekit.net/) in your project with health checks, logging and telemetry.

## Get started

To get started, install the install the [📦 WeChooz.Aspire.MailKit](https://nuget.org/packages/WeChooz.Aspire.MailKit) NuGet package in your application project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package WeChooz.Aspire.MailKit
```
## Usage

In the Program.cs file of your project, call the `AddMailKit` extension method on the application builder to register a `ISmtpClientFactory` for use via the dependency injection container.
This interface defines a async method `GetSmtpClientAsync` to retrieve a ready to use `ISmtpClient`:

```csharp
builder.AddMailKit(builder.Configuration.GetConnectionString("mail"));
```

The integration expect a connection string with the format `smtp://<host>:<port>` or `Endpoint=smtp://<host>:<port>`.
The latter format also allows optional `Username` and `Password` properties to setup authentication to the SMTP server.

## More options
You can specify the settings for the SMTP client integration with the optional `configureSettings` parameter of the `AddMailKit` method. The parameter is action that take the current `MailKitClientSettings`.


Following options are available, in addition of the `Endpoint` and `Credentials` properties:

| Name | Default value | Description |
| --- | --- | --- |
| DisableHealthChecks | `false` | Disables the registration of the health checks for this SMTP client. |
| DisableTracing | `false` | Disables the tracing (logging) for ALL MailKit's SMTP client. |
| DisableMetrics | `false` | Disables the telemetry for ALL MailKit's SMTP client. |

```csharp
builder.AddMailKit(builder.Configuration.GetConnectionString("mail"), settings => { ...});
```

If you need to register multiple SMTP client (to split the load or implement a fallback mechanism),
you can register the SMTP client by specifying a name for the registration before the connection string:

You can also specify a username and a password to secure the connection to the SMTP (local) server (learn [how to work with parameter in .NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/external-parameters)):
```csharp
builder.AddMailKit("fallback", builder.Configuration.GetConnectionString("mail"));
```

## Lovingly inspired by .NET Aspire documentation

This project is lovingly inspired by the [integration samples in the .NET Aspire documentation](https://learn.microsoft.com/en-us/dotnet/aspire/extensibility/custom-client-integration).

It has been improved to meet our internal needs.
