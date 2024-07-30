using Boostlingo.PeopleSoft.Business.Models;
using Boostlingo.PeopleSoft.Resources;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Boostlingo.PeopleSoft.Business.Services
{
    public interface IApiService
    {
        Task<ApiPersonModel[]> FetchData();
    }

    public class ApiService : IApiService
    {
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient;
        private readonly IExceptionService _exceptionService;

        public ApiService(IOptions<AppSettings> appSettings, HttpClient httpClient, IExceptionService exceptionService)
        {
            _appSettings = appSettings.Value;
            _httpClient = httpClient;
            _exceptionService = exceptionService;
        }

        public async Task<ApiPersonModel[]> FetchData()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(_appSettings.DataUrl);
                var result = JsonSerializer.Deserialize<ApiPersonModel[]>(response);
                if (result == null)
                {
                    throw new InvalidOperationException("Deserialization resulted in null.");
                }
                return result;
            }
            catch (Exception ex)
            {
                _exceptionService.LogException(ex);
                return Array.Empty<ApiPersonModel>();
            }
        }
    }
}
