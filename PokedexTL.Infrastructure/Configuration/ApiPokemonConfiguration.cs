namespace PokedexTL.Infrastructure.Configuration;

public class ApiPokemonConfiguration : IApiConfigurationBase
{
    public static readonly string Section =  "PokeApi";
    public string BaseUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Uris { get; set; } = [];
    public string Language { get; set; } = "en";
    // ToDecide: choose whether the default values should be included like this or not
    private const string GetPokemonDefault = "pokemon/";
    private const string GetSpeciesDefault = "pokemon-species/";
    public string GetPokemonBase()
    {
        return this.Uris.GetValueOrDefault("GetPokemon") ?? GetPokemonDefault;
    }
    public string GetSpecies()
    {
        return this.Uris.GetValueOrDefault("GetSpecies")  ?? GetSpeciesDefault;
    }
}