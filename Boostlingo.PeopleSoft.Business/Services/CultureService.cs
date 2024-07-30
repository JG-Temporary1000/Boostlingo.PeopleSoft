using Boostlingo.PeopleSoft.Resources;
using System.Globalization;

namespace Boostlingo.PeopleSoft.Business.Services;

public interface ICultureService
{
    void SetCulture(string cultureCode);
}

public class CultureService : ICultureService
{
    public void SetCulture(string cultureCode)
    {
        var culture = new CultureInfo(cultureCode);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}