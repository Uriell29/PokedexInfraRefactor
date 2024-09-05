using PokeApiNet;
using PokedexAPI_.Models;
using PokedexAPI_.Providers;

namespace PokedexAPI_.Services;

public class PokemonInformationService(ILogger<PokemonInformationService> logger, IPokemonApiClient pokeClient): IPokemonInformationService
{

    public PokemonInformation GetPokemonByName(string name)
    {
        try
        {
            var pokemonName = pokeClient.GetResourceAsync<Pokemon>(name).Result.Name;
            if (pokemonName == null)
            {
                logger.LogInformation("Pokemon with name {name} not found", name);
                return null;
            }
            var pokemonSpecies = pokeClient.GetResourceAsync<PokemonSpecies>(pokemonName).Result;
            return PokemonInformation.MapToPokemonInformation(pokemonName, pokemonSpecies);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while retrieving pokemon.");
            return null;
        }
       
    }
}

