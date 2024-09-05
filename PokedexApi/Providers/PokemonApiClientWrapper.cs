using PokeApiNet;

namespace PokedexAPI_.Providers;

public class PokemonApiClientWrapper(PokeApiClient pokeApiClient) : IPokemonApiClient
{
    public Task<T> GetResourceAsync<T>(string resourceName) where T : NamedApiResource
    {
        return pokeApiClient.GetResourceAsync<T>(resourceName);
    }
}