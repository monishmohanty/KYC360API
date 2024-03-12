using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Serilog;

public class EntityService
{
    private const int MaxRetryAttempts = 3; // Maximum number of retry attempts
    private static readonly TimeSpan InitialRetryDelay = TimeSpan.FromSeconds(1); // Initial delay before the first retry
    private static readonly TimeSpan MaxRetryDelay = TimeSpan.FromSeconds(10); // Maximum delay between retry attempts

    private readonly List<Entity> _entities; // Mock database

    public EntityService()
    {
        // Initialize mock database with sample data
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
            // Add more sample entities here if needed
        };

        // Configure Serilog for logging
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }

    // Method to perform database write operations with retry logic and backoff strategy
    private void PerformWithRetry(Action operation)
    {
        int retryCount = 0;
        TimeSpan retryDelay = InitialRetryDelay;

        while (true)
        {
            try
            {
                operation(); // Perform the database operation
                return; // Operation succeeded, exit the loop
            }
            catch (Exception ex)
            {
                // Log information about the retry attempt
                Log.Error($"Retry attempt {retryCount + 1}: {ex.Message}");

                retryCount++;

                if (retryCount >= MaxRetryAttempts)
                {
                    // Maximum retry attempts reached, rethrow the exception
                    throw;
                }

                // Apply backoff delay before the next retry
                Thread.Sleep(retryDelay);

                // Calculate the next retry delay using exponential backoff
                retryDelay = TimeSpan.FromSeconds(Math.Min(retryDelay.TotalSeconds * 2, MaxRetryDelay.TotalSeconds));
            }
        }
    }

    // CRUD operations...

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
            _entities.Add(entity); // Attempt to add the entity
        });
    }

    public void UpdateEntity(Entity entity)
    {
        PerformWithRetry(() =>
        {
            var existingEntity = _entities.FirstOrDefault(e => e.Id == entity.Id);
            if (existingEntity != null)
            {
                // Update existing entity
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
            _entities.RemoveAll(e => e.Id == id); // Attempt to remove the entity
        });
    }
}
