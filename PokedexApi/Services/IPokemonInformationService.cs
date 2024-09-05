using PokedexAPI_.Models;

namespace PokedexAPI_.Services;

public interface IPokemonInformationService
{
    PokemonInformation GetPokemonByName(string name);
}