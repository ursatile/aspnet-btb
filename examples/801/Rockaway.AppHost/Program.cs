using Projects;

var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Rockaway_WebApp>("webapp");

builder.Build().Run();