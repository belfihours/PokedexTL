using Microsoft.Extensions.Logging;
using PokedexTL.Application.Interfaces;
using PokedexTL.Application.Models;

namespace PokedexTL.Application.Services;

public class PokemonService :  IPokemonService
{
    private readonly IExternalPokemonService _externalPokemonService;
    private readonly ITranslatedPokemonService _translatedPokemonService;
    private readonly ILogger<PokemonService> _logger;
    private const string InvalidPokemonName = "Invalid pokemon name";

    public PokemonService(
        IExternalPokemonService externalPokemonService,
        ITranslatedPokemonService translatedPokemonService,
        ILogger<PokemonService> logger)
    {
        _externalPokemonService = externalPokemonService ??  throw new ArgumentNullException(nameof(externalPokemonService));
        _translatedPokemonService = translatedPokemonService ?? throw new ArgumentNullException(nameof(translatedPokemonService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<PokemonDto> GetPokemonAsync(string pokemonName, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Requested pokemon with name: {PokemonName}", pokemonName);
        if(!IsValidPokemonName(pokemonName))
            throw new ArgumentException(InvalidPokemonName);
        
        return _externalPokemonService.GetPokemonAsync(pokemonName, cancellationToken);
    }

    public async Task<PokemonDto> GetTranslatedPokemonAsync(string pokemonName, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Requested translated pokemon with name: {PokemonName}", pokemonName);
        if(!IsValidPokemonName(pokemonName))
            throw new ArgumentException(InvalidPokemonName);
        
        var pokemon = await _externalPokemonService.GetPokemonAsync(pokemonName, cancellationToken);
        return await _translatedPokemonService.GetTranslatedPokemonAsync(pokemon, cancellationToken);
    }

    private static bool IsValidPokemonName(string pokemonName)
    {
        return !string.IsNullOrWhiteSpace(pokemonName) && pokemonName.All(char.IsLetter);
    }
}