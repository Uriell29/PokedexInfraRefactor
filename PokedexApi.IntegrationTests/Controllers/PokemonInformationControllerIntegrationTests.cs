using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PokeApiNet;
using PokedexAPI_.Models;
using PokedexAPI_.Services;

namespace PokedexApi.IntegrationTests;

public class PokemonInformationControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public PokemonInformationControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.UseTestServer();
        });
        _client = _factory.CreateClient();
    }

    [Theory]
    [InlineData("pikachu")]
    [InlineData("bulbasaur")]
    public async Task GetPokemonInformation_ShouldReturnsOk_WithPokemonInformation(string pokemonName)
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/pokemon/{pokemonName}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pokemon = await response.Content.ReadFromJsonAsync<PokemonInformation>();
        Assert.NotNull(pokemon);
        Assert.Equal(pokemonName, pokemon.Name);
    }

    [Fact]
    public async Task GetPokemonInformation_ShouldReturnNotFound_WhenPokemonDoesNotExist()
    {
        // Arrange
        var pokemonName = "NotFound";

        // Act
        var response = await _client.GetAsync($"/api/v1/pokemon/{pokemonName}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    

    [Fact]
    public async Task GetTranslatedPokemonInformation_ShouldReturnOk_WithTranslatedInformation()
    {
        //Arrange
        var pokemonName = "mewtwo";

        //Act
        var response = await _client.GetAsync($"/api/v1/pokemon/translated/{pokemonName}");

        //Assert
        response.EnsureSuccessStatusCode();
        var pokemon = await response.Content.ReadFromJsonAsync<PokemonInformation>();
        Assert.NotNull(pokemon);
        Assert.Equal(pokemonName, pokemon.Name);
        Assert.NotNull(pokemon.Description);
    }
    
    [Fact]
    public async Task GetTranslatedPokemonInformation_ShouldReturnNotFound_WhenPokemonDoesNotExist()
    {
        // Arrange
        var pokemonName = "nonexistent";

        // Act
        var response = await _client.GetAsync($"/api/v1/pokemon/translated/{pokemonName}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetPokemonInformation_ShouldReturnInternalServerError_WhenServiceThrowsException()
    {
        // Arrange
        var pokemonName = "pikachu";

        var clientWithException = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                var mockService = new Mock<IPokemonInformationService>();
                mockService.Setup(service => service.GetPokemonByNameAsync(It.IsAny<string>()))
                    .ThrowsAsync(new Exception("Simulated exception"));

                services.AddSingleton(mockService.Object);
            });
        }).CreateClient();

        // Act
        var response = await clientWithException.GetAsync($"/api/v1/pokemon/{pokemonName}");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Equal("Internal server error", errorMessage);
    }
}