using System.Text.Json.Serialization;

namespace PokedexTL.Infrastructure.ExternalModels;

public record ExternalPokemon
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
};