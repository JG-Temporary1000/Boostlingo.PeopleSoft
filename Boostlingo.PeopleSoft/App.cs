using Boostlingo.PeopleSoft.Business.Models;
using Boostlingo.PeopleSoft.Business.Services;
using Boostlingo.PeopleSoft.Resources;

namespace Boostlingo.PeopleSoft;

public class App
{
    private readonly AppSettings _appSettings;
    private readonly IConsoleService _consoleService;
    private readonly ICultureService _cultureService;

    public App(IConsoleService consoleService, ICultureService cultureService, AppSettings appSettings)
    {
        _consoleService = consoleService;
        _cultureService = cultureService;
        _appSettings = appSettings;
    }

    public async void Run()
    {
        _cultureService.SetCulture(_appSettings.DefaultCulture);
        _consoleService.DisplayAscii();
        _consoleService.SelectLanguage();
        _consoleService.WriteConsole(Translations.WelcomeUser, true, ConsoleColor.Green);
        var option = ProcessOptionEnum.NotSet;
        while (option != ProcessOptionEnum.Exit)
        {
            option = _consoleService.MainMenu();
            if (option != ProcessOptionEnum.Exit) await _consoleService.RunProcessAsync(option);
        }
        _consoleService.WriteConsole(Translations.Goodbye, true, ConsoleColor.Green);
        Environment.Exit(0);
    }
}
