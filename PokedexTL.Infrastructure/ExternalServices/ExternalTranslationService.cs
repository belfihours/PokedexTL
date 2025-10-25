using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokedexTL.API.Configuration;
using PokedexTL.Application.Interfaces;
using PokedexTL.Infrastructure.Configuration;
using PokedexTL.Infrastructure.ExternalModels;

namespace PokedexTL.Infrastructure.ExternalServices;

public class ExternalTranslationService : IExternalTranslationService
{
    private readonly HttpClient _httpClient;
    private readonly ApiTranslatorConfiguration  _configuration;
    private readonly ILogger<ExternalTranslationService> _logger;

    public ExternalTranslationService(
        HttpClient httpClient,
        IOptions<ApiTranslatorConfiguration> configuration,
        ILogger<ExternalTranslationService> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration?.Value);
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<string> GetTranslationWithYodaAsync(string text, CancellationToken cancellationToken)
    {
        return await GetTranslationWith(text, _configuration.GetYoda(), cancellationToken);
    }

    public async Task<string> GetTranslationWithShakespeareAsync(string text, CancellationToken cancellationToken)
    {
        return await GetTranslationWith(text, _configuration.GetShakespeare(), cancellationToken);
    }
    
    private async Task<string> GetTranslationWith(
        string text,
        string translationLanguage,
        CancellationToken cancellationToken)
    {
        var bodyContent = GetBodyContent(text);
        var response = 
            await _httpClient.PostAsync($"{translationLanguage}", bodyContent, cancellationToken);
        try
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var translationResponse = JsonSerializer.Deserialize<TranslationResponse>(content);

            return translationResponse?.Success.Total > 0 
                ? translationResponse.Content.Translated 
                : text;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("Unexpected Http error while translating text: {Exception}", ex.Message);
            return text;
        }
    }
    
    private static StringContent GetBodyContent(string text)
    {
        var body = GetTranslationBody(text);
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return content;
    }

    private static object GetTranslationBody(string text)
    {
        return new { text = text };
    }
}