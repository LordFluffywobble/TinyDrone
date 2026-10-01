var builder = DistributedApplication.CreateBuilder(args);

var dbPassword= builder.AddParameter("Gjetterdudette", secret: true);

var postgresServer = builder.AddPostgres("postgres", password: dbPassword)
                            .WithContainerName("mitt-drone-prosjekt-db")
                            .WithDataVolume("drone-db-lagring")
                            .WithPgAdmin();

// Opprett databasen
var droneDb = postgresServer.AddDatabase("dronedb");

//Start webapi og gi tilgang til databasen
builder.AddProject("webapi-service", "../TinyDrone.WebApi/TinyDrone.WebApi.csproj")
       .WithReference(droneDb)
       .WaitFor(droneDb);

builder.Build().Run();