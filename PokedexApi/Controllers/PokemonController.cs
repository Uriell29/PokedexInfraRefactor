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
    public IActionResult GetPokemonInformation(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Pokemon name is required.");
            }

            var pokemon = pokemonInformationService.GetPokemonByName(name);
            return pokemon == null ? NotFound() : Ok(pokemon);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while retrieving pokemon.");
            return StatusCode(500, "Internal server error");
        }
    }
}