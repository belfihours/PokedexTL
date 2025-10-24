using Microsoft.Extensions.Logging;
using PokedexTL.Application.Interfaces;
using PokedexTL.Application.Models;

namespace PokedexTL.Application.Services;

public class PokemonService :  IPokemonService
{
    private readonly ILogger<PokemonService> _logger;

    public PokemonService(ILogger<PokemonService> logger)
    {
        _logger = logger;
    }

    public Task<PokemonDto> GetPokemonAsync(string name)
    {
        _logger.LogInformation("Get pokemon called");
        return Task.FromResult(new PokemonDto(name + "ax", "desc", "hab", true));
    }
}