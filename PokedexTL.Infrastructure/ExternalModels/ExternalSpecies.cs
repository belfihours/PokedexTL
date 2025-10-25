using System.Text.Json.Serialization;

namespace PokedexTL.Infrastructure.ExternalModels;

public record ExternalSpecies
{
    [JsonPropertyName("flavor_text_entries")]
    public required FlavorTextEntry[] FlavorTextEntries { get; init; }
    [JsonPropertyName("habitat")]
    public required HabitatEntry Habitat { get; init; }
    [JsonPropertyName("is_legendary")]
    public required bool IsLegendary { get; init; }
};

public record FlavorTextEntry
{
    [JsonPropertyName("flavor_text")]
    public required string Text { get; init; }
    [JsonPropertyName("language")]
    public required LanguageEntry Language { get; init; }
}

public record LanguageEntry
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}

public record HabitatEntry
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}