using System.Text.Json.Serialization;

namespace PokedexTL.Infrastructure.Configuration;

public class ExternalApisConfiguration
{
    public static readonly string Section =  "ExternalApis";
    public ApiPokemonConfiguration PokeApi { get; init; } = new();
    public ApiTranslatorConfiguration TranslatorApi { get; init; }= new();
}