using Microsoft.AspNetCore.Mvc;
using PokedexAPI_.Models;
using PokedexAPI_.Services;

namespace PokedexAPI_.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PokemonController(ILogger<PokemonController> logger, IPokemonInformationService pokemonInformationService)
    : ControllerBase
{
    [HttpGet("{name}")]
    [ProducesResponseType<PokemonInformation>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPokemonInformationAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return BadRequest("Pokemon name is required.");

        try
        {
            var pokemon = await pokemonInformationService.GetPokemonByNameAsync(name);
            return pokemon == null ? NotFound() : Ok(pokemon);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while retrieving pokemon.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("translated/{name}")]
    [ProducesResponseType<PokemonInformation>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTranslatedPokemonInformationAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return BadRequest("Pokemon name is required.");

        try
        {
            var pokemon = await pokemonInformationService.GetPokemonWithTranslatedDescriptionByNameAsync(name);
            return pokemon == null ? NotFound() : Ok(pokemon);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while retrieving pokemon with translated description.");
            return StatusCode(500, "Internal server error");
        }
    }
}