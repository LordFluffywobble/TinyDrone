using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using TinyDrone.TinyDrone.WebApi.Models;
public class MyDbContext : DbContext{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options){}

    public DbSet<Drone> Drones => Set<Drone>();
}



