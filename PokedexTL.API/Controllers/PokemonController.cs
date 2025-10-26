using System.Net;
using Microsoft.AspNetCore.Mvc;
using PokedexTL.Application.Interfaces;
using PokedexTL.Application.Models;
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
    
    /// <summary>
    /// Endpoint to get basic Pokémon information
    /// </summary>
    /// <param name="pokemonName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{pokemonName}")]
    [ProducesResponseType(typeof(PokemonDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetPokemon(
        [FromRoute] string pokemonName,
        CancellationToken cancellationToken)
    {
        var result = await _pokemonService.GetPokemonAsync(pokemonName, cancellationToken);
        return Ok(result);
    }
    
    /// <summary>
    /// Endpoint to get basic Pokémon information with translated description in Yoda or Shakespeare language
    /// </summary>
    /// <param name="pokemonName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    // ToDecide: maybe add used language to translate in the response
    [HttpGet]
    [Route("translated/{pokemonName}")]
    [ProducesResponseType(typeof(PokemonDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetTranslatedPokemon(
        [FromRoute] string pokemonName,
        CancellationToken cancellationToken)
    {
        var result = await _pokemonService.GetTranslatedPokemonAsync(pokemonName, cancellationToken);
        return Ok(result);
    }
}