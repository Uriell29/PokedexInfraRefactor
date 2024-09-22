using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PokedexAPI_.Controllers;
using PokedexAPI_.Models;
using PokedexAPI_.Services;

namespace PokedexApi.Tests;

public class PokemonControllerTest
{
    private readonly Mock<ILogger<PokemonController>> _mockLogger;
    private readonly Mock<IPokemonInformationService> _mockPokemonService;
    private readonly PokemonController _sut;

    public PokemonControllerTest()
    {
        _mockLogger = new Mock<ILogger<PokemonController>>();
        _mockPokemonService = new Mock<IPokemonInformationService>();
        _sut = new PokemonController(_mockLogger.Object, _mockPokemonService.Object);
    }

    [Fact]
    public async Task GetPokemonInformation_ShouldReturnHttpOkAndPokemonInformation_WhenPokemonExists()
    {
        //Arrange
        var expectedResult = new PokemonInformation("name", "description", "habitat", false);
        _mockPokemonService.Setup(m => m.GetPokemonByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedResult);
        //Act
        var result = await _sut.GetPokemonInformationAsync("name") as OkObjectResult;

        //Assert
        Assert.Equal(200, result.StatusCode);
        result.Value.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetPokemonInformation_ShouldReturnHttpNotFound_WhenPokemonDoesNotExist()
    {
        _mockPokemonService.Setup(m => m.GetPokemonByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((PokemonInformation)null);

        var result = await _sut.GetPokemonInformationAsync("nameThatDoesNotExist") as NotFoundResult;
        
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task GetPokemonInformation_ShouldReturnBadRequest_WhenNameIsInvalid(string invalidName)
    {
        var result = await _sut.GetPokemonInformationAsync(invalidName) as BadRequestObjectResult;
        
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task GetPokemonInformation_ShouldReturnHttpInternalServerError_WhenExceptionIsThrown()
    {
        _mockPokemonService.Setup(m => m.GetPokemonByNameAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _sut.GetPokemonInformationAsync("name") as ObjectResult;
        
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
        _mockLogger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while retrieving pokemon.")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task GetTranslatedPokemonInformation_ShouldReturnHttpOkAndPokemonInformation_WhenPokemonExists()
    {
        //Arrange
        var expectedPokemon = new PokemonInformation("name", "description", "habitat", false);
        _mockPokemonService.Setup(m => m.GetPokemonWithTranslatedDescriptionByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedPokemon);
        //Act
        var result = await _sut.GetTranslatedPokemonInformationAsync("name") as OkObjectResult;

        //Assert
        Assert.Equal(200, result.StatusCode);
        result.Value.Should().BeEquivalentTo(expectedPokemon);
    }

    [Fact]
    public async Task GetTranslatedPokemonInformation_ShouldReturnHttpNotFound_WhenPokemonDoesNotExist()
    {
        //Arrange
        _mockPokemonService.Setup(m => m.GetPokemonWithTranslatedDescriptionByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((PokemonInformation?)null);

        //Act
        var result = await _sut.GetTranslatedPokemonInformationAsync("name") as NotFoundResult;
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task GetTranslatedPokemonInformationAsync_ShouldReturnBadRequest_WhenNameIsInvalid(string invalidName)
    {
        // Act
        var result = await _sut.GetTranslatedPokemonInformationAsync(invalidName) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        result.Value.Should().Be("Pokemon name is required.");
    }

    [Fact]
    public async Task GetTranslatedPokemonInformation_ShouldReturnHttpInternalServerError_WhenExceptionIsThrown()
    {
        //Arrange
        _mockPokemonService.Setup(m => m.GetPokemonWithTranslatedDescriptionByNameAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        //Act
        var result = await _sut.GetTranslatedPokemonInformationAsync("name") as ObjectResult;
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
        _mockLogger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while retrieving pokemon with translated description.")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }
}