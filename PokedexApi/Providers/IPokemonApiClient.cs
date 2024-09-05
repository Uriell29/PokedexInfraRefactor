using PokeApiNet;

namespace PokedexAPI_.Providers;

public interface IPokemonApiClient
{
    Task<T> GetResourceAsync<T>(string resourceName) where T : NamedApiResource;
}