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

        var result = await _sut.GetPokemonInformationAsync("name") as NotFoundResult;
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetPokemonInformation_ShouldReturnBadRequest_WhenNameParameterIsMissing()
    {
        var result = await _sut.GetPokemonInformationAsync(null) as BadRequestResult;
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPokemonInformation_ShouldReturnHttpInternalServerError_WhenExceptionIsThrown()
    {
        _mockPokemonService.Setup(m => m.GetPokemonByNameAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _sut.GetPokemonInformationAsync("name") as ObjectResult;
        Assert.Equal(500, result.StatusCode);
    }

    [Fact]
    public async Task GetTranslatedPokemonInformation_ShouldReturnHttpOkAndPokemonInformation_WhenPokemonExists()
    {
        //Arrange
        var expectedResult = new PokemonInformation("name", "description", "habitat", false);
        var pokemon = new PokemonInformation("name", "description", "habitat", false);
        _mockPokemonService.Setup(m => m.GetPokemonWithTranslatedDescriptionByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(pokemon);
        //Act
        var result = await _sut.GetTranslatedPokemonInformationAsync("name") as OkObjectResult;

        //Assert
        Assert.Equal(200, result.StatusCode);
        result.Value.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetTranslatedPokemonInformation_ShouldReturnHttpNotFound_WhenPokemonDoesNotExist()
    {
        _mockPokemonService.Setup(m => m.GetPokemonWithTranslatedDescriptionByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((PokemonInformation?)null);

        var result = await _sut.GetTranslatedPokemonInformationAsync("name") as NotFoundResult;
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task GetTranslatedPokemonInformation_ShouldReturnHttpInternalServerError_WhenExceptionIsThrown()
    {
        _mockPokemonService.Setup(m => m.GetPokemonWithTranslatedDescriptionByNameAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Test exception"));

        var result = await _sut.GetTranslatedPokemonInformationAsync("name") as ObjectResult;
        Assert.Equal(500, result.StatusCode);
    }
}