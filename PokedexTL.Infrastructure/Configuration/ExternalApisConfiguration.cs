using PokedexTL.Infrastructure.Configuration;

namespace PokedexTL.API.Configuration;

public class ExternalApisConfiguration
{
    public static readonly string Section =  "ExternalApis";
    public readonly ApiPokemonConfiguration ApiPokemonConfiguration = new();
    public readonly ApiTranslatorConfiguration ApiTranslatorConfiguration = new();
}