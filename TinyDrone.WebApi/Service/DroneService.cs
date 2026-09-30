using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using TinyDrone.TinyDrone.WebApi.Models;

namespace TinyDrone.TinyDrone.WebApi.Services;

public class DroneService(MyDbContext db){
    
    public async Task<Drone> CreateDroneAsync(DroneDTO dto){
        var newDrone = new Drone(0, dto.Name, dto.Speed, dto.X, dto.Y, dto.Z, dto.Status);

        db.Drones.Add(newDrone);
        await db.SaveChangesAsync();
        return newDrone;
    }
    public async Task<List<Drone>> DroneListAsync(){
        return await db.Drones.ToListAsync();
    }
    public async Task<Drone?> GetDroneByIdAsync(int id){ //Make sure Drone can be nullable for the endpoint logic to work
       return await db.Drones.FindAsync(id);
    }
    public async Task<Drone?>DeleteDroneByIdAsync(int id){ //Can not use id directly for some reason
        var drone = await db.Drones.FindAsync(id);
        if (drone == null) return null;

        db.Drones.Remove(drone);
        await db.SaveChangesAsync();

        return drone;
    }
    public async Task<Drone?>UpdateDroneWithIdAsync(int id, DroneDTO dto){
        var drone = await db.Drones.FindAsync(id);
        if (drone == null) return null;

        var updatedDrone = drone with{
          Name  = dto.Name,
          Speed = dto.Speed,
          X     = dto.X,
          Y     = dto.Y,
          Z     = dto.Z,
          Status= dto.Status  
        };
        db.Entry(drone).CurrentValues.SetValues(updatedDrone);

        await db.SaveChangesAsync();

        return updatedDrone;
    }
    public async Task<(string Name1, string Name2, double Distance)?>ShortestDronePathAsync(int id1, int id2){
        var drone1 = await db.Drones.FindAsync(id1);
        var drone2 = await db.Drones.FindAsync(id2);
        if (drone1 == null || drone2 == null) return null;
            
        double dx = drone2.X - drone1.X;
        double dy = drone2.Y - drone1.Y;
        double dz = drone2.Z - drone1.Z;

        double distance = Math.Sqrt(dx*dx + dy*dy + dz*dz);

        return new (drone1.Name, drone2.Name, distance);

        //return Math.Sqrt(dx*dx + dy*dy + dz*dz);
    }
}


    
