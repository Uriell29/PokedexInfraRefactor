using PokeApiNet;
using PokedexAPI_.Models;
using PokedexAPI_.Providers;

namespace PokedexAPI_.Services;

public class PokemonInformationService(
    ILogger<IPokemonInformationService> logger,
    IPokemonApiClient pokeClient,
    IFunTranslationApiClient funTranslationApiClient)
    : IPokemonInformationService
{
    public async Task<PokemonInformation?> GetPokemonByNameAsync(string name)
    {
        try
        {
            return await BuildPokemonInformationAsync(name);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while retrieving pokemon.");
            return null;
        }
    }

    public async Task<PokemonInformation?> GetPokemonWithTranslatedDescriptionByNameAsync(string name)
    {
        try
        {
            var pokemonInformation = await BuildPokemonInformationAsync(name);
            if (pokemonInformation is null)
                return null;

            string translatedDescription;
            try
            {
                translatedDescription = await GetTranslatedDescriptionAsync(pokemonInformation);
            }
            catch (FunTranslationApiException ex)
            {
                logger.LogError(ex, "Translation API failed for Pokémon {Name}. Returning standard description.", name);
                return pokemonInformation;
            }

            return new PokemonInformation(
                pokemonInformation.Name,
                translatedDescription,
                pokemonInformation.Habitat,
                pokemonInformation.IsLegendary);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while retrieving translated Pokémon {Name}.", name);
            return null;
        }
    }

    private async Task<string> GetTranslatedDescriptionAsync(PokemonInformation pokemonInfo)
    {
        if (pokemonInfo.IsLegendary || pokemonInfo.Habitat == "cave")
            return await funTranslationApiClient.GetYodaTranslationAsync(pokemonInfo.Description);

        return await funTranslationApiClient.GetShakespeareTranslationAsync(pokemonInfo.Description);
    }


    private async Task<PokemonInformation?> BuildPokemonInformationAsync(string name)
    {
        try
        {
            var pokemon = await pokeClient.GetResourceAsync<Pokemon>(name);
            if (pokemon?.Name == null)
            {
                logger.LogInformation("Pokemon with name {Name} not found", name);
                return null;
            }

            var pokemonSpecies = await pokeClient.GetResourceAsync<PokemonSpecies>(pokemon.Species.Name);
            return PokemonInformation.MapToPokemonInformation(pokemon.Name, pokemonSpecies);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error building Pokémon information for {Name}", name);
            return null;
        }
    }
}