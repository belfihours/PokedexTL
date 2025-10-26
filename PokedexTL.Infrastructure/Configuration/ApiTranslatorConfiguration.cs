namespace PokedexTL.Infrastructure.Configuration;

public class ApiTranslatorConfiguration : IApiConfigurationBase
{
    public static readonly string Section =  "TranslatorApi";
    public string BaseUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Uris { get; set; } = [];
    private const string GetYodaDefault = "yoda.json";
    private const string GetShakespeareDefault = "shakespeare.json";

    public string GetYoda()
    {
        return this.Uris.GetValueOrDefault("GetYoda") ?? GetYodaDefault;
    }
    public string GetShakespeare()
    {
        return this.Uris.GetValueOrDefault("GetShakespeare")  ?? GetShakespeareDefault;
    }
}