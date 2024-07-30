using Microsoft.EntityFrameworkCore;
using Boostlingo.PeopleSoft.Data.Contexts;
using Boostlingo.PeopleSoft.Data.Entities;
using Boostlingo.PeopleSoft.Business.Services;
using NUnit.Framework;

namespace Boostlingo.PeopleSoft.Tests
{
    public class DataServiceTests
    {
        private ApplicationDbContext _context;
        private IDataService _dataService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "boostlingo-peoplesoft")
                .Options;

            _context = new ApplicationDbContext(options);
            _dataService = new DataService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetPersonsAsync_ReturnsAllPersons()
        {
            // Arrange
            _context.Persons.AddRange(new List<Person>
            {
                new Person { Id = "1", FirstName = "John", LastName = "Doe", Bio = "John's background.", Version = 1, Language = "en" },
                new Person { Id = "2", FirstName = "Jane", LastName = "Doe", Bio = "Jane's past.", Version = 2, Language = "fr" }
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _dataService.GetPersonsAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].FirstName, Is.EqualTo("John"));
            Assert.That(result[1].FirstName, Is.EqualTo("Jane"));
        }

        [Test]
        public async Task LoadPersonsAsync_AddsSinglePerson()
        {
            // Arrange
            var person = new Person { Id = "1", FirstName = "John", LastName = "Doe", Bio = "Bio", Version = 1, Language = "en" };

            // Act
            await _dataService.LoadPersonsAsync(new List<Person> { person }, 1);

            // Assert
            var personsInDb = await _context.Persons.ToListAsync();
            Assert.That(personsInDb.Count, Is.EqualTo(1));
            Assert.That(personsInDb[0].FirstName, Is.EqualTo("John"));
        }
        
        [Test]
        public async Task AddOrUpdatePersonAsync_AddsSinglePerson()
        {
            // Arrange
            var person = new Person { Id = "IAKPO3R4761JDRVG", FirstName = "Aamir", LastName = "Dilmurat", Language = "Sindhi", Bio = "Vestibulum pharetra libero et velit gravida euismod. Quisque mauris ligula, efficitur porttitor sodales ac, lacinia non ex. Fusce eu ultrices elit, vel posuere neque.", Version = 7.27 };

            // Act
            await _dataService.AddOrUpdatePersonAsync(person);

            // Assert
            var personsInDb = await _context.Persons.ToListAsync();
            Assert.That(personsInDb.Count, Is.EqualTo(1));
            Assert.That(personsInDb[0].FirstName, Is.EqualTo("Aamir"));
        }

        [Test]
        public async Task LoadPersonsAsync_AddsPersonsInBatches()
        {
            // Arrange
            var newPersons = new List<Person>
            {
                new Person { Id = "V59OF92YF627HFY0", FirstName = "Adeel", LastName = "Solangi", Language = "Sindhi", Bio = "Donec lobortis eleifend condimentum. Cras dictum dolor lacinia lectus vehicula rutrum. Maecenas quis nisi nunc. Nam tristique feugiat est vitae mollis. Maecenas quis nisi nunc.", Version =  6.1 },
                new Person { Id = "ENTOCR13RSCLZ6KU", FirstName = "Afzal", LastName = "Ghaffar", Language = "Sindhi", Bio = "Aliquam sollicitudin ante ligula, eget malesuada nibh efficitur et. Pellentesque massa sem, scelerisque sit amet odio id, cursus tempor urna. Etiam congue dignissim volutpat. Vestibulum pharetra libero et velit gravida euismod.", Version =  1.88 },
                new Person { Id = "IAKPO3R4761JDRVG", FirstName = "Aamir", LastName = "Dilmurat", Language = "Sindhi", Bio = "Vestibulum pharetra libero et velit gravida euismod. Quisque mauris ligula, efficitur porttitor sodales ac, lacinia non ex. Fusce eu ultrices elit, vel posuere neque.", Version =  7.27 },
            };

            // Act
            await _dataService.LoadPersonsAsync(newPersons, 1);

            // Assert
            var personsInDb = await _context.Persons.ToListAsync();
            Assert.That(personsInDb.Count, Is.EqualTo(newPersons.Count));
            Assert.That(personsInDb[0].FirstName, Is.EqualTo("Adeel"));
            Assert.That(personsInDb[1].FirstName, Is.EqualTo("Afzal"));
            Assert.That(personsInDb[2].FirstName, Is.EqualTo("Aamir"));
        }

        [Test]
        public async Task AddOrUpdatePersonsAsync_AddsOrUpdatesPersonsCorrectly()
        {
            // Arrange
            var newPersons = new List<Person>
            {
                new Person { Id = "V59OF92YF627HFY0", FirstName = "Adeel", LastName = "Solangi", Language = "Sindhi", Bio = "Donec lobortis eleifend condimentum. Cras dictum dolor lacinia lectus vehicula rutrum. Maecenas quis nisi nunc. Nam tristique feugiat est vitae mollis. Maecenas quis nisi nunc.", Version =  6.1 },
                new Person { Id = "ENTOCR13RSCLZ6KU", FirstName = "Afzal", LastName = "Ghaffar", Language = "Sindhi", Bio = "Aliquam sollicitudin ante ligula, eget malesuada nibh efficitur et. Pellentesque massa sem, scelerisque sit amet odio id, cursus tempor urna. Etiam congue dignissim volutpat. Vestibulum pharetra libero et velit gravida euismod.", Version =  1.88 },
                new Person { Id = "IAKPO3R4761JDRVG", FirstName = "John", LastName = "Doe", Language = "English", Bio = "Updated bio.", Version = 8.0 },
            };

            var existingPerson = new Person { Id = "IAKPO3R4761JDRVG", FirstName = "Aamir", LastName = "Dilmurat", Language = "Sindhi", Bio = "Vestibulum pharetra libero et velit gravida euismod. Quisque mauris ligula, efficitur porttitor sodales ac, lacinia non ex. Fusce eu ultrices elit, vel posuere neque.", Version = 7.27 };
            await _context.Persons.AddAsync(existingPerson);
            await _context.SaveChangesAsync();

            // Act
            await _dataService.AddOrUpdatePersonsAsync(newPersons);

            // Assert
            var personsInDb = await _context.Persons.ToListAsync();
            Assert.That(personsInDb.Count, Is.EqualTo(newPersons.Count));

            var person1 = personsInDb.First(p => p.Id == "V59OF92YF627HFY0");
            var person2 = personsInDb.First(p => p.Id == "ENTOCR13RSCLZ6KU");
            var person3 = personsInDb.First(p => p.Id == "IAKPO3R4761JDRVG");

            // Check updated person
            Assert.That(person1.FirstName, Is.EqualTo("Adeel"));
            Assert.That(person1.LastName, Is.EqualTo("Solangi"));
            Assert.That(person1.Bio, Is.EqualTo("Donec lobortis eleifend condimentum. Cras dictum dolor lacinia lectus vehicula rutrum. Maecenas quis nisi nunc. Nam tristique feugiat est vitae mollis. Maecenas quis nisi nunc."));
            Assert.That(person1.Version, Is.EqualTo(6.1));
            Assert.That(person1.Language, Is.EqualTo("Sindhi"));

            // Check new person
            Assert.That(person2.FirstName, Is.EqualTo("Afzal"));
            Assert.That(person2.LastName, Is.EqualTo("Ghaffar"));
            Assert.That(person2.Bio, Is.EqualTo("Aliquam sollicitudin ante ligula, eget malesuada nibh efficitur et. Pellentesque massa sem, scelerisque sit amet odio id, cursus tempor urna. Etiam congue dignissim volutpat. Vestibulum pharetra libero et velit gravida euismod."));
            Assert.That(person2.Version, Is.EqualTo(1.88));
            Assert.That(person2.Language, Is.EqualTo("Sindhi"));

            // Check update person
            Assert.That(person3.FirstName, Is.EqualTo("John"));
            Assert.That(person3.LastName, Is.EqualTo("Doe"));
            Assert.That(person3.Bio, Is.EqualTo("Updated bio."));
            Assert.That(person3.Language, Is.EqualTo("English"));
            Assert.That(person3.Version, Is.EqualTo(8.0));
        }
    }
}
