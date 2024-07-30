using Boostlingo.PeopleSoft.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Boostlingo.PeopleSoft.Data.Contexts;

public class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
    {
    }
}
