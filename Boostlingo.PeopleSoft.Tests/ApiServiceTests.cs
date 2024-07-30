using Boostlingo.PeopleSoft.Business.Models;
using Boostlingo.PeopleSoft.Business.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Net;
using System.Text.Json;

namespace Boostlingo.PeopleSoft.Tests.Services;

public class ApiServiceTests
{
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private Mock<IExceptionService> _exceptionServiceMock;
    private IOptions<AppSettings> _appSettingsOptions;
    private HttpClient _httpClient;
    private ApiService _apiService;

    [SetUp]
    public void SetUp()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _exceptionServiceMock = new Mock<IExceptionService>();
        _appSettingsOptions = Options.Create(new AppSettings
        {
            DataUrl = "https://microsoftedge.github.io/Demos/json-dummy-data/64KB.json",
            DefaultCulture = "en"
        });
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _apiService = new ApiService(_appSettingsOptions, _httpClient, _exceptionServiceMock.Object);
    }

    [Test]
    public async Task FetchData_ReturnsExpectedData_WhenHttpRequestIsSuccessful()
    {
        // Arrange
        var expectedData = new[]
        {
            new ApiPersonModel {
                Name = "Adeel Solangi",
                Language = "Sindhi",
                Id = "V59OF92YF627HFY0",
                Bio = "Donec lobortis eleifend condimentum. Cras dictum dolor lacinia lectus vehicula rutrum. Maecenas quis nisi nunc. Nam tristique feugiat est vitae mollis. Maecenas quis nisi nunc.",
                Version = 6.1
            },
            new ApiPersonModel {
                Name = "Afzal Ghaffar",
                Language = "Sindhi",
                Id = "ENTOCR13RSCLZ6KU",
                Bio = "Aliquam sollicitudin ante ligula, eget malesuada nibh efficitur et. Pellentesque massa sem, scelerisque sit amet odio id, cursus tempor urna. Etiam congue dignissim volutpat. Vestibulum pharetra libero et velit gravida euismod.",
                Version = 1.88
            },
            new ApiPersonModel {
                Name = "Aamir Solangi",
                Language = "Sindhi",
                Id = "IAKPO3R4761JDRVG",
                Bio = "Vestibulum pharetra libero et velit gravida euismod. Quisque mauris ligula, efficitur porttitor sodales ac, lacinia non ex. Fusce eu ultrices elit, vel posuere neque.",
                Version = 7.27
            },
            new ApiPersonModel {
                Name = "Abla Dilmurat",
                Language = "Uyghur",
                Id = "5ZVOEPMJUI4MB4EN",
                Bio = "Donec lobortis eleifend condimentum. Morbi ac tellus erat.",
                Version = 2.53
            },
        };
        var jsonResponse = JsonSerializer.Serialize(expectedData);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _apiService.FetchData();

        // Assert
        Assert.That(result.Length, Is.EqualTo(expectedData.Length));
        Assert.That(result[0].Id, Is.EqualTo(expectedData[0].Id));
        Assert.That(result[1].Id, Is.EqualTo(expectedData[1].Id));
        Assert.That(result[2].Id, Is.EqualTo(expectedData[2].Id));
        Assert.That(result[3].Id, Is.EqualTo(expectedData[3].Id));
    }

    [Test]
    public async Task FetchData_ThrowsInvalidOperationException_WhenDeserializationResultsInNull()
    {
        // Arrange
        var jsonResponse = string.Empty;

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _apiService.FetchData();

        // Assert
        _exceptionServiceMock.Verify(es => es.LogException(It.IsAny<Exception>()), Times.Once);
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task FetchData_ReturnsEmptyArray_AndLogsException_WhenHttpRequestFails()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Request failed"));

        // Act
        var result = await _apiService.FetchData();

        // Assert
        _exceptionServiceMock.Verify(es => es.LogException(It.IsAny<Exception>()), Times.Once);
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task FetchData_ReturnsEmptyArray_AndLogsException_WhenDeserializationFails()
    {
        // Arrange
        var invalidJsonResponse = "{ bad JSON goes here }";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(invalidJsonResponse)
            });

        // Act
        var result = await _apiService.FetchData();

        // Assert
        _exceptionServiceMock.Verify(es => es.LogException(It.IsAny<Exception>()), Times.Once);
        Assert.IsEmpty(result);
    }
}

