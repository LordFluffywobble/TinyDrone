using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Scalar.AspNetCore;
using TinyDrone.TinyDrone.WebApi.Models;
using TinyDrone.TinyDrone.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>{
    options.Authority = builder.Configuration["Authentications: Authority"];
    options.Audience  = builder.Configuration["Authentication: Audience"];
    options.RequireHttpsMetadata = builder.Environment.IsDevelopment();
});

builder.Services.AddAuthorization();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<DroneService>();

builder.AddNpgsqlDbContext<MyDbContext>("dronedb");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db =  scope.ServiceProvider.GetRequiredService<MyDbContext>();

    await db.Database.EnsureCreatedAsync();

    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("TinyDrone API")
               .WithTheme(ScalarTheme.DeepSpace)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options.AddPreferredSecuritySchemes("OAuth2");

        options.AddAuthorizationCodeFlow("OAuth2", flow =>{
            flow.ClientId = "ditt-klient-her";
            flow.AuthorizationUrl = "https://din-identity-provider.com";
            flow.TokenUrl = "https://din-identity-provider.com";
            flow.Pkce = Pkce.Sha256;
        });
    });
}

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapPost("/api/drones/createdrone", async (DroneDTO dto, DroneService droneService) =>
        Results.Created($"/api/drones/createdrone", await droneService.CreateDroneAsync(dto)))
        .RequireAuthorization();

    app.MapGet("/api/drones/list", async (DroneService droneService) =>
        Results.Ok(await droneService.DroneListAsync()))
        .RequireAuthorization();

    app.MapGet("/api/drone/{id}", async (int id, DroneService droneService) =>{
        var drone  = await droneService.GetDroneByIdAsync(id);
        if (drone == null) return TypedResults.NotFound($"No drone with ID {id} exists");
        
        return Results.Ok(drone);
    }).RequireAuthorization();
    
    app.MapDelete("/api/deletedrone/{id}", async (int id, DroneService droneService) =>{
        var deletedDrone  = await droneService.DeleteDroneByIdAsync(id); 
        if (deletedDrone == null){
            return TypedResults.NotFound($"Found no drone with ID {id} to delete");
        }
        return Results.Ok(new {
            Message = "Drone deleted successfully",
            Drone   = deletedDrone
        });
    })
    .RequireAuthorization();
    
    app.MapPut("/api/updatedrone/{id}", async (int id, DroneDTO dto, DroneService dService) =>{
        var updatedDrone  = await dService.UpdateDroneWithIdAsync(id, dto); 
        if (updatedDrone == null){
            return TypedResults.NotFound($"No drone with ID {id}");
        }
        return Results.Ok(new{
            Message = $"Drone {id} updated successfully",
            Drone   = updatedDrone    
        });
    })
    .RequireAuthorization();
    
    app.MapGet("/api/findshortestpath/{id1}/{id2}", async (int id1, int id2, DroneService dService) => {
        var sPath = await dService.ShortestDronePathAsync(id1, id2);
            if (sPath == null) {
                return TypedResults.NotFound($"Error: Missing one or more drones to calculate the shortest path between them");
        }
        return Results.Ok(new{
            Message = $"The shortest path between {sPath.Value.Name1} and {sPath.Value.Name2} is:",
            Path = sPath.Value.Distance
        });
    })
    .RequireAuthorization();


app.Run();