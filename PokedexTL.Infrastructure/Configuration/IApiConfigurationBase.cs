namespace PokedexTL.Infrastructure.Configuration;

public interface IApiConfigurationBase
{
    string BaseUrl { get; set; }
    Dictionary<string,string> Uris { get; set; }
}