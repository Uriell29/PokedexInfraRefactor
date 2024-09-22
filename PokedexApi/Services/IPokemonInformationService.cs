using PokedexAPI_.Models;

namespace PokedexAPI_.Services;

public interface IPokemonInformationService
{
    Task<PokemonInformation?> GetPokemonByNameAsync(string name);

    Task<PokemonInformation?> GetPokemonWithTranslatedDescriptionByNameAsync(string name);
}