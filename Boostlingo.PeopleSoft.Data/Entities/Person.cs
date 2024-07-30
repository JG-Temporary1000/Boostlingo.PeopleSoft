using System.ComponentModel.DataAnnotations;

namespace Boostlingo.PeopleSoft.Data.Entities;

public class Person
{
    [Key]
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Language { get; set; }
    public required string Bio { get; set; }
    public required double Version { get; set; }
}
