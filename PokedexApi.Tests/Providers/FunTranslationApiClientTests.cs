using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PokedexAPI_.Providers;

namespace PokedexAPI.Tests.Providers;

public class FunTranslationApiClientTests
{
    private static FunTranslationApiClient _client;
    private static Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private static Mock<ILogger<FunTranslationApiClient>> _loggerMock;

    public FunTranslationApiClientTests()
    {
        _loggerMock = new Mock<ILogger<FunTranslationApiClient>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _client = new FunTranslationApiClient(_loggerMock.Object, httpClient);
    }


    [Fact]
    public async Task GetYodaTranslation_ShouldReturnTranslatedDescription_WhenApiResponseIsValid()
    {
        // Arrange
        var sourceDescription = "Test description";
        var yodaTranslatedDescription = "Test translated text";
        var jsonResponse = $"{{\"contents\": {{\"translated\": \"{yodaTranslatedDescription}\"}}}}";

        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _client.GetYodaTranslationAsync(sourceDescription);

        // Assert
        Assert.Equal(yodaTranslatedDescription, result);
    }

    [Fact]
    public async Task GetYodaTranslation_ShouldReturnDefaultMessage_WhenTranslationIsNotAvailable()
    {
        // Arrange
        var sourceDescription = "Test description";
        var jsonResponse = "{}";

        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _client.GetYodaTranslationAsync(sourceDescription);

        // Assert
        Assert.Equal("Translation not available.", result);
    }

    [Fact]
    public async Task GetYodaTranslation_ShouldThrowAnExceptionAndLogsError_WhenApiCallFails()
    {
        // Arrange
        var sourceDescription = "Test description";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new FunTranslationApiException("An unexpected error occurred while getting yoda translation."));

        // Act & Assert
        await Assert.ThrowsAsync<FunTranslationApiException>(() =>
            _client.GetYodaTranslationAsync(sourceDescription));
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains("An unexpected error occurred while getting yoda translation.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task GetShakespeareTranslation_ShouldReturnTranslatedText_WhenApiResponseIsValid()
    {
        // Arrange
        var sourceDescription = "Test description";
        var translatedText = "Test translated text";
        var jsonResponse = $"{{\"contents\": {{\"translated\": \"{translatedText}\"}}}}";

        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _client.GetShakespeareTranslationAsync(sourceDescription);

        // Assert
        Assert.Equal(translatedText, result);
    }

    [Fact]
    public async Task GetShakespeareTranslation_ShouldReturnDefaultMessage_WhenTranslationNotAvailable()
    {
        // Arrange
        var sourceDescription = "Test description";
        var jsonResponse = "{}";

        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _client.GetShakespeareTranslationAsync(sourceDescription);

        // Assert
        Assert.Equal("Translation not available.", result);
    }

    [Fact]
    public async Task GetShakespeareTranslation_ShouldThrowAnExceptionAndLogsError_WhenApiCallFails()
    {
        // Arrange
        var sourceDescription = "Test description";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new FunTranslationApiException("An unexpected error occurred while getting shakespeare translation."));

        // Act & Assert
        await Assert.ThrowsAsync<FunTranslationApiException>(() =>
            _client.GetShakespeareTranslationAsync(sourceDescription));
    
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains("An unexpected error occurred while getting shakespeare translation.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }


    private static void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
    }
}