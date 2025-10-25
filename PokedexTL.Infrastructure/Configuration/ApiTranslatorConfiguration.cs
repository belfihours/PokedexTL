namespace PokedexTL.Infrastructure.Configuration;

public class ApiTranslatorConfiguration : IApiConfigurationBase
{
    public static readonly string Section =  "TranslatorApi";
    private const string GetYodaDefault = "yoda.json";
    private const string GetShakespeareDefault = "shakespeare.json";

    public string GetPokemonBase()
    {
        return this.Uris.GetValueOrDefault("GetYoda") ?? GetYodaDefault;
    }
    public string GetSpecies()
    {
        return this.Uris.GetValueOrDefault("GetShakespeare")  ?? GetShakespeareDefault;
    }

    public string BaseUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Uris { get; set; } = [];
}