using System.ComponentModel.DataAnnotations;
using PokeApiNet;

namespace PokedexAPI_.Models;

public class PokemonInformation(string name, string description, string habitat, bool isLegendary)
{
    [Required] public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public string Habitat { get; set; } = habitat;
    public bool IsLegendary { get; set; } = isLegendary;


    public static PokemonInformation MapToPokemonInformation(string pokemonName, PokemonSpecies pokemonSpecies) =>
        new(
            pokemonName,
            pokemonSpecies.FlavorTextEntries == null
                ? null
                : pokemonSpecies.FlavorTextEntries.FirstOrDefault()?.FlavorText.Replace("\n", "") ?? null,
            pokemonSpecies.Habitat == null ? null : pokemonSpecies.Habitat.Name,
            pokemonSpecies.IsLegendary);
}