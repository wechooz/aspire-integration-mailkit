var builder = DistributedApplication.CreateBuilder(args);


var mail = builder.AddMailDev("mail");

builder.AddProject<Projects.WebApplication>("webapplication")
    .WithReference(mail).WaitFor(mail);

builder.Build().Run();
