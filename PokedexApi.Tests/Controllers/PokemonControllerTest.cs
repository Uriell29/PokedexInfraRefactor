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
    public void ShouldReturnHttpOkAndPokemonInformationIfPokemonExists()
    {
        //Arrange
        var expectedResult = new PokemonInformation("name", "description", "habitat", false);
        var pokemon = new PokemonInformation("name", "description", "habitat", false);
        _mockPokemonService.Setup(m => m.GetPokemonByName(It.IsAny<string>()))
            .Returns(pokemon);
        //Act
        var result = _sut.GetPokemonInformation("name") as OkObjectResult;

        //Assert
        Assert.Equal(200, result.StatusCode);
        result.Value.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ShouldReturnHttpNotFoundIfPokemonDoesNotExist()
    {
        _mockPokemonService.Setup(m => m.GetPokemonByName(It.IsAny<string>()))
            .Returns((PokemonInformation)null);

        var result = _sut.GetPokemonInformation("name") as NotFoundResult;
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public void ShouldReturnBadRequest_IfNameParameterIsMissing()
    {
        var result = _sut.GetPokemonInformation(null) as BadRequestResult;
        Assert.Null(result);
    }
}