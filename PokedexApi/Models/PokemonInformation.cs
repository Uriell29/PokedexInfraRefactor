using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using PokeApiNet;

namespace PokedexAPI_.Models;

public class PokemonInformation(string name, string description, string habitat, bool isLegendary)
{
    [Required] public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public string Habitat { get; set; } = habitat;
    public bool IsLegendary { get; set; } = isLegendary;


    public static PokemonInformation MapToPokemonInformation(string pokemonName, PokemonSpecies pokemonSpecies)
    {
        return new PokemonInformation(
            pokemonName,
            GetDescription(pokemonSpecies),
            pokemonSpecies.Habitat == null ? null : pokemonSpecies.Habitat.Name,
            pokemonSpecies.IsLegendary);
    }

    private static string? GetDescription(PokemonSpecies pokemonSpecies)
    {
        return pokemonSpecies.FlavorTextEntries == null
            ? null
            : CleanUpUnicodeCharacters(pokemonSpecies.FlavorTextEntries.FirstOrDefault()?.FlavorText);
    }

    private static string CleanUpUnicodeCharacters(string sentence)
    {
        return Regex.Replace(sentence, @"\p{C}", " ");
    }
}