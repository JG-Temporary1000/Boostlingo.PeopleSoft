using Boostlingo.PeopleSoft.Business.Models;
using Boostlingo.PeopleSoft.Data.Entities;
using Boostlingo.PeopleSoft.Resources;
using System.Data;

namespace Boostlingo.PeopleSoft.Business.Services
{
    public interface IConsoleService
    {
        void DisplayAscii();
        void SelectLanguage();
        void WriteConsole(string message, bool injectBreak, ConsoleColor color);
        ProcessOptionEnum MainMenu();
        Task RunProcessAsync(ProcessOptionEnum option);
    }

    public class ConsoleService : IConsoleService
    {
        private readonly ICultureService _cultureService;
        private readonly IApiService _apiService;
        private readonly IDataService _dataService;
        private readonly IExceptionService _exceptionService;
        private const string AsciiArt = @"
            ,---------------------------,
            |  /---------------------\  |
            | |                       | |
            | |      Boostlingo       | |
            | |      PeopleSoft       | |
            | |         v1.0          | |
            | |                       | |
            |  \_____________________/  |
            |___________________________|
          ,---\_____     []     _______/------,
        /         /______________\           /|
      /___________________________________ /  | ___
      |                                   |   |    )
      |  o o o                 [-------]  |  /    (__
      |__________________________________ |/     /  /
      /-------------------------------------/|  ( )/
    /-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/ /
  /-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/ /
  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";

        public ConsoleService(ICultureService cultureService, IApiService apiService, IDataService dataService, IExceptionService exceptionService)
        {
            _cultureService = cultureService;
            _apiService = apiService;
            _dataService = dataService;
            _exceptionService = exceptionService;
        }

        public void DisplayAscii()
        {
            Console.Clear();
            WriteConsole(AsciiArt);
        }

        public ProcessOptionEnum MainMenu()
        {
            try
            {
                var menuOptions = new Dictionary<int, ProcessOptionEnum>
                {
                    { 1, ProcessOptionEnum.LoadData },
                    { 2, ProcessOptionEnum.SortData },
                    { 3, ProcessOptionEnum.Exit }
                };

                while (true)
                {
                    WriteConsole("#################################", true);
                    WriteConsole(Translations.MainMenu);
                    WriteConsole("#################################", false);
                    foreach (var option in menuOptions)
                    {
                        WriteConsole($"{option.Key}. {GetOptionDescription(option.Value)}");
                    }
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"\n  {Translations.NumberSelection} ");
                    Console.ResetColor();
                    string input = Console.ReadLine() ?? string.Empty;

                    if (int.TryParse(input, out int selectedOption) && menuOptions.TryGetValue(selectedOption, out var processOption))
                    {
                        return processOption;
                    }
                    else
                    {
                        WriteConsole(Translations.InvalidSelection, true, ConsoleColor.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteConsole(Translations.OopsMessage, true, ConsoleColor.Red);
                _exceptionService.LogException(ex);
                return ProcessOptionEnum.NotSet;
            }
        }

        public async Task RunProcessAsync(ProcessOptionEnum option)
        {
            try
            {
                switch (option)
                {
                    case ProcessOptionEnum.LoadData:
                        await LoadDataAsync();
                        break;
                    case ProcessOptionEnum.SortData:
                        await SortDataAsync();
                        break;
                    case ProcessOptionEnum.Exit:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(option), option, "Invalid process option.");
                }
            }
            catch (Exception ex)
            {
                WriteConsole(Translations.OopsMessage, true, ConsoleColor.Red);
                _exceptionService.LogException(ex);
            }
        }

        public void SelectLanguage()
        {
            try
            {
                var languageOptions = new Dictionary<int, string> {{ 1, "en" }, { 2, "es" }, { 3, "fr" }};
                var languageNames = new Dictionary<int, string> { { 1, Translations.English }, { 2, Translations.Spanish }, { 3, Translations.French }};
                while (true)
                {
                    WriteConsole($"{Translations.SelectLanguage}...", true);
                    foreach (var option in languageOptions)
                    {
                        WriteConsole($"{option.Key}. {languageNames[option.Key]} ({option.Value})");
                    }
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"\n  {Translations.NumberSelection} ");
                    Console.ResetColor();
                    string input = Console.ReadLine() ?? string.Empty;

                    if (int.TryParse(input, out int selectedLanguage) && languageOptions.ContainsKey(selectedLanguage))
                    {
                        _cultureService.SetCulture(languageOptions[selectedLanguage]);
                        break;
                    } else {
                        WriteConsole(Translations.InvalidSelection, true, ConsoleColor.Red);
                    }
                }
            }
            catch (Exception eX)
            {
                WriteConsole(Translations.OopsMessage, true, ConsoleColor.Red);
                _exceptionService.LogException(eX);
            }
        }

        public void WriteConsole(string message, bool injectBreak = false, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            if (injectBreak) Console.WriteLine();
            Console.WriteLine($"  {message}");
            Console.ResetColor();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Get the data from URL
                var data = await _apiService.FetchData();

                // Make sure we got some data back
                if (data.Length == 0) {
                    throw new InvalidOperationException("The API returned no data.");
                }

                var persons = data.Select(person =>
                {
                    var names = ParseFullName(person.Name);
                    return new Person
                    {
                        Id = person.Id,
                        Bio = person.Bio,
                        Language = person.Language,
                        Version = person.Version,
                        FirstName = names.Length > 0 ? names[0] : string.Empty,
                        LastName = names.Length > 1 ? names[1] : string.Empty
                    };
                }).ToList();

                await _dataService.AddOrUpdatePersonsAsync(persons);
                WriteConsole(Translations.DataLoaded, true, ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                WriteConsole(Translations.OopsMessage, true, ConsoleColor.Red);
                _exceptionService.LogException(ex);
            }
        }

        private static string[] ParseFullName(string fullName)
        {
            return fullName.Split(' ', 2); // Split into first and last name
        }

        private static string GetOptionDescription(ProcessOptionEnum option)
        {
            return option switch
            {
                ProcessOptionEnum.LoadData => Translations.OptionLoadData,
                ProcessOptionEnum.SortData => Translations.OptionSortData,
                ProcessOptionEnum.Exit => Translations.OptionExit,
                _ => throw new ArgumentOutOfRangeException(nameof(option), option, "Invalid process option.")
            };
        }

        private async Task SortDataAsync()
        {
            try
            {
                var persons = await _dataService.GetPersonsAsync();
                if (persons.Any())
                {
                    foreach (var person in persons.OrderBy(x => x.LastName).ThenBy(x => x.FirstName))
                    {
                        Console.WriteLine($"  {person.LastName}, {person.FirstName}");
                    }
                }
                else
                {
                    WriteConsole(Translations.NoData, true, ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                WriteConsole(Translations.OopsMessage, true, ConsoleColor.Red);
                _exceptionService.LogException(ex);
            }
        }
    }
}
