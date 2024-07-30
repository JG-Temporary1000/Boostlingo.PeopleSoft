using Boostlingo.PeopleSoft.Data.Contexts;
using Boostlingo.PeopleSoft.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boostlingo.PeopleSoft.Business.Services;
public interface IDataService
{
    /// <remarks>
    /// Some methods within the DataService class are not currently in use.
    /// These methods were implemented to explore various strategies for data loading,
    /// considering factors such as load times and concurrent access.
    /// </remarks>
    Task LoadPersonsAsync(List<Person> persons, int batchSize);
    Task DeletePersonsAsync();
    Task AddOrUpdatePersonAsync(Person person);
    Task AddOrUpdatePersonsAsync(List<Person> persons);
    Task<List<Person>> GetPersonsAsync();
}

public class DataService : IDataService
{
    private readonly ApplicationDbContext _context;

    public DataService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task DeletePersonsAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM Persons");
    }

    public async Task AddOrUpdatePersonsAsync(List<Person> persons)
    {
        // Get all existing persons in a single query
        var personIds = persons.Select(p => p.Id).ToList();
        var existingPersons = await _context.Persons.Where(p => personIds.Contains(p.Id)).ToListAsync();

        // Create dictionaries to track updates and inserts used later in batching
        var personsExisting = existingPersons.ToDictionary(p => p.Id);
        var personsToUpdate = new List<Person>();
        var personsToInsert = new List<Person>();

        foreach (var person in persons)
        {
            if (personsExisting.TryGetValue(person.Id, out var existingPerson))
            {
                // Update existing person
                existingPerson.FirstName = person.FirstName;
                existingPerson.LastName = person.LastName;
                existingPerson.Language = person.Language;
                existingPerson.Bio = person.Bio;
                existingPerson.Version = person.Version;
                personsToUpdate.Add(existingPerson);
            }
            else
            {
                // Add new person
                personsToInsert.Add(person);
            }
        }

        // Batch update and insert operations
        if (personsToUpdate.Any())
        {
            _context.Persons.UpdateRange(personsToUpdate);
        }

        if (personsToInsert.Any())
        {
            await _context.Persons.AddRangeAsync(personsToInsert);
        }

        // Save to database
        await _context.SaveChangesAsync();
    }

    public async Task LoadPersonsAsync(List<Person> persons, int batchSize)
    {
        try
        {
            for (int i = 0; i < persons.Count; i += batchSize)
            {
                var batch = persons.Skip(i).Take(batchSize).ToList();
                _context.Persons.AddRange(batch);
                await _context.SaveChangesAsync();
            }
        }
        finally
        {
            _context.ChangeTracker.Clear();
        }
    }

    public async Task AddOrUpdatePersonAsync(Person person)
    {
        var existingPerson = await _context.Persons.FindAsync(person.Id);
        if (existingPerson != null)
        {
            // Update existing person
            existingPerson.FirstName = person.FirstName;
            existingPerson.LastName = person.LastName;
            existingPerson.Language = person.Language;
            existingPerson.Bio = person.Bio;
            existingPerson.Version = person.Version;
            _context.Persons.Update(existingPerson);
        }
        else
        {
            // Add new person
            await _context.Persons.AddAsync(person);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<Person>> GetPersonsAsync()
    {
        return await _context.Persons.ToListAsync();
    }
}
