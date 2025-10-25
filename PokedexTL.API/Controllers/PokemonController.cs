using Microsoft.AspNetCore.Mvc;
using PokedexTL.Application.Interfaces;
using PokedexTL.Application.Services;

namespace PokedexTL.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PokemonController : Controller
{
    private readonly IPokemonService _pokemonService;
    public PokemonController(IPokemonService pokemonService)
    {
        _pokemonService = pokemonService ?? throw new ArgumentNullException(nameof(pokemonService));
    }
    
    // GET
    [HttpGet]
    [Route("{pokemonName}")]
    public async Task<IActionResult> TestController(
        [FromRoute] string pokemonName,
        CancellationToken cancellationToken)
    {
        var result = await _pokemonService.GetPokemonAsync(pokemonName);
        return Ok(result);
    }
}