using PokeApiNet;
using PokedexAPI_.Contexts;
using PokedexAPI_.Models;
using PokedexAPI_.Providers;
using PokedexAPI_.Strategies;

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

            var translatedDescription = await GetTranslatedDescriptionAsync(pokemonInformation, name);
            if (translatedDescription is null)
                return pokemonInformation;

            return new PokemonInformation(
                pokemonInformation.Name,
                translatedDescription,
                pokemonInformation.Habitat,
                pokemonInformation.IsLegendary);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while getting Pokémon {Name}.", name);
            return null;
        }
    }

    private async Task<PokemonInformation?> BuildPokemonInformationAsync(string name)
    {
        try
        {
            var pokemon = await pokeClient.GetResourceAsync<Pokemon>(name);
            if (pokemon.Name == null)
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

    private async Task<string?> GetTranslatedDescriptionAsync(PokemonInformation pokemonInfo, string name)
    {
        var translationContext = new TranslationContext();
        SetTranslationStrategy(pokemonInfo, translationContext);

        try
        {
            return await translationContext.TranslateDescriptionAsync(pokemonInfo.Description);
        }
        catch (FunTranslationApiException e)
        {
            logger.LogError(e, "Translation API failed for Pokémon {Name}. Returning standard description.", name);
            return null;
        }
    }

    private void SetTranslationStrategy(PokemonInformation pokemonInfo, TranslationContext context)
    {
        if (pokemonInfo.IsLegendary || pokemonInfo.Habitat == "cave")
            context.SetTranslationStrategy(new YodaTranslationStrategy(funTranslationApiClient));
        else
            context.SetTranslationStrategy(new ShakespeareTranslationStrategy(funTranslationApiClient));
    }
}