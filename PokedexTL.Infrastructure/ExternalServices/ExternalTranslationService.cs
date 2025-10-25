using PokedexTL.Application.Interfaces;

namespace PokedexTL.Infrastructure.ExternalServices;

public class ExternalTranslationService : IExternalTranslationService
{
    private readonly HttpClient _httpClient;

    public ExternalTranslationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public Task<string> GetTranslationWithYodaAsync(string text, CancellationToken cancellationToken)
    {
        return Task.FromResult(text + "Yoda");
    }

    public Task<string> GetTranslationWithShakespeareAsync(string text, CancellationToken cancellationToken)
    {
        return Task.FromResult(text + "Shakespeare");
    }
}