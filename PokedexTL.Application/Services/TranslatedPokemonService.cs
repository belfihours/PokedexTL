using Microsoft.Extensions.Logging;
using PokedexTL.Application.Interfaces;
using PokedexTL.Application.Models;

namespace PokedexTL.Application.Services;

public class TranslatedPokemonService : ITranslatedPokemonService
{
    private readonly IExternalTranslationService _translationService;
    private readonly ILogger<PokemonService> _logger;
    private const string YodaTranslationHabitat = "cave";

    public TranslatedPokemonService(IExternalTranslationService translationService, ILogger<PokemonService> logger)
    {
        _translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PokemonDto> GetTranslatedPokemonAsync(PokemonDto pokemon, CancellationToken cancellationToken)
    {
        var translatedDescription = await GetDescription(pokemon, cancellationToken);
        _logger.LogInformation("Translation complete. From {oldText} to {newText}"
            , pokemon.Description
            , translatedDescription);
        return pokemon with { Description = translatedDescription };
    }

    private async Task<string> GetDescription(PokemonDto pokemon, CancellationToken cancellationToken)
    {
        if (IsEligibleToYodaTranslation(pokemon))
            return await _translationService.GetTranslationWithYodaAsync(pokemon.Description, cancellationToken);

        return await _translationService.GetTranslationWithShakespeareAsync(pokemon.Description, cancellationToken);
    }

    private static bool IsEligibleToYodaTranslation(PokemonDto pokemon)
    {
        return pokemon.Habitat == YodaTranslationHabitat || pokemon.IsLegendary;
    }
}