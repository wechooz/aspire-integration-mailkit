using WeChooz.Aspire.MailKit;

var builder = WebApplication.CreateBuilder(args);

builder.AddMailKit(builder.Configuration.GetConnectionString("mail") ?? throw new ArgumentNullException("connectionstring_mail"));
var app = builder.Build();

app.MapGet("/", async (ISmtpClientFactory smtpClientFactory) =>
{
    var smtpClient = await smtpClientFactory.GetSmtpClientAsync();
    // Send mail via "await smtpClient.SendAsync(...);"

    return "Hello: " + smtpClient.GetType().FullName;
});

// Adds the health check endpoint
app.MapHealthChecks("/health");

await app.RunAsync();
