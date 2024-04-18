var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.aspire_ApiService>("apiservice");

builder.AddProject<Projects.aspire_Web>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();
