using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using PokeApiNet;

namespace PokedexApi.IntegrationTests;

public class PokemonInformationControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PokemonInformationControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.UseTestServer();
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ShouldReturnsOk_WithPokemonInformation()
    {
        // Arrange
        var pokemonName = "pikachu";

        // Act
        var response = await _client.GetAsync($"/api/pokemon/{pokemonName}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pokemon = response.Content.ReadFromJsonAsync<Pokemon>();
        Assert.NotNull(pokemon);
        Assert.Equal("pikachu", pokemon.Result.Name);
    }

    [Fact]
    public async Task ShouldReturnsNotFound()
    {
        // Arrange
        var pokemonName = "NotFound";

        // Act
        var response = await _client.GetAsync($"/api/pokemon/{pokemonName}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}