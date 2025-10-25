using System.Text.Json.Serialization;

namespace PokedexTL.Infrastructure.ExternalModels;

public record TranslationResponse
{
    [JsonPropertyName("success")]
    public required TranslationSuccess Success { get; init; }
    [JsonPropertyName("contents")]
    public required TranslationContents Content { get; init; }
}

public record TranslationSuccess
{
    [JsonPropertyName("total")]
    public required int Total { get; init; }
}

public record TranslationContents
{
    [JsonPropertyName("translated")]
    public required string Translated { get; init; }
    [JsonPropertyName("text")]
    public required string OriginalText { get; init; }
    [JsonPropertyName("translation")]
    public required string TranslationLanguage { get; init; }
}