using Microsoft.Extensions.Logging;
using Moq;
using PokeApiNet;
using PokedexAPI_.Providers;
using PokedexAPI_.Services;

namespace PokedexAPI.Tests.Services;

public class PokemonInformationServiceTest
{
    private readonly Mock<ILogger<PokemonInformationService>> _mockLogger;
    private readonly Mock<IPokemonApiClient> _mockPokeApiClient;
    private PokemonInformationService _sut;

    public PokemonInformationServiceTest()
    {
        _mockLogger = new Mock<ILogger<PokemonInformationService>>();
        _mockPokeApiClient = new Mock<IPokemonApiClient>();
        _sut = new PokemonInformationService(_mockLogger.Object, _mockPokeApiClient.Object);
    }
    
    [Fact]
    public void GetPokemonByName_ShouldReturnCorrectPokemon()
    {
        //Arrange
        const string pokemonName = "APokemonName";
        var pokemon = new Pokemon
        {
            Name = pokemonName
        };
        var pokemonSpecies = new PokemonSpecies
        {
            Name = pokemonName
        };
        _mockPokeApiClient.Setup(m => m.GetResourceAsync<Pokemon>(It.IsAny<string>()))
            .ReturnsAsync(pokemon);
        _mockPokeApiClient.Setup(m => m.GetResourceAsync<PokemonSpecies>(It.IsAny<string>()))
            .ReturnsAsync(pokemonSpecies);
        
        //Act
        var result = _sut.GetPokemonByName("APokemonName");
        
        //Assert
        Assert.Equal(pokemonName, result.Name);
    }
    
    [Fact]
    public void GetPokemonByName_ShouldReturnNull_IfPokemonNotFound()
    {
        //Arrange
        const string pokemonName = "APokemonName";
        _mockPokeApiClient.Setup(m => m.GetResourceAsync<Pokemon>(pokemonName))
            .ThrowsAsync(new Exception());
        
        //Act
        var result = _sut.GetPokemonByName("APokemonName");
        
        //Assert
        Assert.Null(result);
    }
}