var builder = DistributedApplication.CreateBuilder(args);

var postgresServer = builder.AddPostgres("postgres")
                            .WithPgAdmin();

// Opprett databasen
var droneDb = postgresServer.AddDatabase("dronedb");

//Start webapi og gi tilgang til databasen
builder.AddProject("webapi-service", "../TinyDrone.WebApi/TinyDrone.WebApi.csproj")
       .WithReference(droneDb);

builder.Build().Run();