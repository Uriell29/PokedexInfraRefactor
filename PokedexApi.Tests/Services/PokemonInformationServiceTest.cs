using Microsoft.Extensions.Logging;
using Moq;
using PokeApiNet;
using PokedexAPI_.Models;
using PokedexAPI_.Providers;
using PokedexAPI_.Services;

namespace PokedexAPI.Tests.Services;

public class PokemonInformationServiceTest
{
    private readonly Mock<IFunTranslationApiClient> _mockFunTranslationApiClient;
    private readonly Mock<ILogger<PokemonInformationService>> _mockLogger;
    private readonly Mock<IPokemonApiClient> _mockPokeApiClient;
    private readonly PokemonInformationService _sut;

    public PokemonInformationServiceTest()
    {
        _mockLogger = new Mock<ILogger<PokemonInformationService>>();
        _mockPokeApiClient = new Mock<IPokemonApiClient>();
        _mockFunTranslationApiClient = new Mock<IFunTranslationApiClient>();
        _sut = new PokemonInformationService(_mockLogger.Object, _mockPokeApiClient.Object,
            _mockFunTranslationApiClient.Object);
    }

    [Fact]
    public void GetPokemonByName_ShouldReturnCorrectPokemon_WhenPokemonExists()
    {
        // Arrange
        const string pokemonName = "pikachu";
        SetupPokemonMock(pokemonName, "forest", false, "An electric Pokémon.");

        // Act
        var result = _sut.GetPokemonByNameAsync(pokemonName);

        // Assert
        VerifyPokemonInfo(result.Result, pokemonName, "forest", false, "An electric Pokémon.");
    }

    [Fact]
    public void GetPokemonByName_ShouldReturnNull_WhenPokemonNotFound()
    {
        // Arrange
        const string pokemonName = "nonexistent";
        _mockPokeApiClient.Setup(m => m.GetResourceAsync<Pokemon>(pokemonName)).ThrowsAsync(new Exception());

        // Act
        var result = _sut.GetPokemonByNameAsync(pokemonName);

        // Assert
        Assert.Null(result.Result);
    }

    [Fact]
    public void GetPokemonByName_ShouldReturnNull_WhenPokemonSpeciesNotFound()
    {
        // Arrange
        const string pokemonName = "speciesNotFound";
        var pokemon = new Pokemon
            { Name = pokemonName, Species = new NamedApiResource<PokemonSpecies> { Name = "missing_species" } };

        _mockPokeApiClient.Setup(m => m.GetResourceAsync<Pokemon>(pokemonName)).ReturnsAsync(pokemon);
        _mockPokeApiClient.Setup(m => m.GetResourceAsync<PokemonSpecies>(pokemon.Species.Name))
            .ThrowsAsync(new Exception("Species not found"));

        // Act
        var result = _sut.GetPokemonByNameAsync(pokemonName);

        // Assert
        Assert.Null(result.Result);
    }

    [Theory]
    [InlineData("zubat", "A bat-like Pokémon.", "cave", false, "A bat-like Pokémon, it is.")]
    [InlineData("mewtwo", "A legendary psychic Pokémon.", "unknown", true, "A legendary psychic Pokémon, it is.")]
    public void
        GetPokemonWithTranslatedDescriptionByName_ShouldReturnYodaTranslatedDescription_ForCaveOrLegendaryPokemon(
            string name, string originalDescription, string habitat, bool isLegendary, string yodaDescription)
    {
        // Arrange
        SetupPokemonMock(name, habitat, isLegendary, originalDescription);
        _mockFunTranslationApiClient.Setup(client => client.GetYodaTranslationAsync(originalDescription))
            .ReturnsAsync(yodaDescription);

        // Act
        var result = _sut.GetPokemonWithTranslatedDescriptionByNameAsync(name);

        // Assert
        VerifyPokemonInfo(result.Result, name, habitat, isLegendary, yodaDescription);
    }

    [Fact]
    public void
        GetPokemonWithTranslatedDescriptionByName_ShouldReturnShakespeareTranslatedDescription_WhenNotLegendaryAndNotCave()
    {
        // Arrange
        var pokemonName = "pikachu";
        var originalDescription = "A small electric Pokémon.";
        var shakespeareDescription = "A small electric Pokémon, verily.";

        SetupPokemonMock(pokemonName, "forest", false, originalDescription);
        _mockFunTranslationApiClient.Setup(client => client.GetShakespeareTranslationAsync(originalDescription))
            .ReturnsAsync(shakespeareDescription);

        // Act
        var result = _sut.GetPokemonWithTranslatedDescriptionByNameAsync(pokemonName);

        // Assert
        VerifyPokemonInfo(result.Result, pokemonName, "forest", false, shakespeareDescription);
    }

    [Fact]
    public void GetPokemonWithTranslatedDescriptionByName_ShouldReturnStandardDescription_WhenFunApiIsDown()
    {
        // Arrange
        var pokemonName = "charizard";
        var originalDescription = "A dragon-like fire Pokémon.";

        SetupPokemonMock(pokemonName, "mountain", false, originalDescription);
        _mockFunTranslationApiClient.Setup(client => client.GetShakespeareTranslationAsync(It.IsAny<string>()))
            .Throws(new FunTranslationApiException("FunTranslation API is down"));

        // Act
        var result = _sut.GetPokemonWithTranslatedDescriptionByNameAsync(pokemonName);

        // Assert
        VerifyPokemonInfo(result.Result, pokemonName, "mountain", false, originalDescription);
    }

    private void SetupPokemonMock(string name, string habitat, bool isLegendary, string description)
    {
        var pokemon = new Pokemon
        {
            Name = name,
            Species = new NamedApiResource<PokemonSpecies>()
        };

        var pokemonSpecies = new PokemonSpecies
        {
            Name = name,
            FlavorTextEntries = new List<PokemonSpeciesFlavorTexts>
            {
                new() { FlavorText = description }
            },
            Habitat = new NamedApiResource<PokemonHabitat> { Name = habitat },
            IsLegendary = isLegendary
        };

        _mockPokeApiClient.Setup(m => m.GetResourceAsync<Pokemon>(It.IsAny<string>())).ReturnsAsync(pokemon);
        _mockPokeApiClient.Setup(m => m.GetResourceAsync<PokemonSpecies>(It.IsAny<string>()))
            .ReturnsAsync(pokemonSpecies);
    }

    private void VerifyPokemonInfo(PokemonInformation? result, string name, string habitat, bool isLegendary,
        string description)
    {
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(habitat, result.Habitat);
        Assert.Equal(isLegendary, result.IsLegendary);
        Assert.Equal(description, result.Description);
    }
}