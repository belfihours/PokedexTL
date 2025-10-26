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
    public async Task<IActionResult> GetPokemon(
        [FromRoute] string pokemonName,
        CancellationToken cancellationToken)
    {
        var result = await _pokemonService.GetPokemonAsync(pokemonName, cancellationToken);
        return Ok(result);
    }
    
    // GET
    // ToDecide: maybe add used language to translate in the response
    [HttpGet]
    [Route("translated/{pokemonName}")]
    public async Task<IActionResult> GetTranslatedPokemon(
        [FromRoute] string pokemonName,
        CancellationToken cancellationToken)
    {
        var result = await _pokemonService.GetTranslatedPokemonAsync(pokemonName, cancellationToken);
        return Ok(result);
    }
}