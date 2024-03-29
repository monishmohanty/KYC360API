using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Serilog;

public class EntityService
{
    private const int MaxRetryAttempts = 3; 
    private static readonly TimeSpan InitialRetryDelay = TimeSpan.FromSeconds(1); 
    private static readonly TimeSpan MaxRetryDelay = TimeSpan.FromSeconds(10); 

    private readonly List<Entity> _entities; 

    public EntityService()
    {
        
        _entities = new List<Entity>
        {
            new Entity
            {
                Id = "1",
                Names = new List<Name>
                {
                    new Name { FirstName = "Monish", Surname = "Mohanty" }
                },
                Addresses = new List<Address>
                {
                    new Address { AddressLine = "123 Main ", City = "Bangalore", Country = "India" }
                },
                Dates = new List<Date>
                {
                    new Date { DateType = "Birth", DateValue = new DateTime(2003, 4, 7) }
                },
                Gender = "Male",
                Deceased = false
            },
            
        };

        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }

    
    private void PerformWithRetry(Action operation)
    {
        int retryCount = 0;
        TimeSpan retryDelay = InitialRetryDelay;

        while (true)
        {
            try
            {
                operation(); 
                return; 
            }
            catch (Exception ex)
            {
                
                Log.Error($"Retry attempt {retryCount + 1}: {ex.Message}");

                retryCount++;

                if (retryCount >= MaxRetryAttempts)
                {
                    
                    throw;
                }

                
                Thread.Sleep(retryDelay);

                
                retryDelay = TimeSpan.FromSeconds(Math.Min(retryDelay.TotalSeconds * 2, MaxRetryDelay.TotalSeconds));
            }
        }
    }

    

    public IEnumerable<Entity> GetAllEntities()
    {
        return _entities;
    }

    public Entity GetEntityById(string id)
    {
        return _entities.FirstOrDefault(e => e.Id == id);
    }

    public void AddEntity(Entity entity)
    {
        PerformWithRetry(() =>
        {
            _entities.Add(entity); 
        });
    }

    public void UpdateEntity(Entity entity)
    {
        PerformWithRetry(() =>
        {
            var existingEntity = _entities.FirstOrDefault(e => e.Id == entity.Id);
            if (existingEntity != null)
            {
                
                existingEntity.Names = entity.Names;
                existingEntity.Addresses = entity.Addresses;
                existingEntity.Dates = entity.Dates;
                existingEntity.Gender = entity.Gender;
                existingEntity.Deceased = entity.Deceased;
            }
        });
    }

    public void DeleteEntity(string id)
    {
        PerformWithRetry(() =>
        {
            _entities.RemoveAll(e => e.Id == id); 
        });
    }
}
