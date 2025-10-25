using Microsoft.Extensions.DependencyInjection;
using PokedexTL.Application.Interfaces;
using PokedexTL.Infrastructure.ExternalServices;

namespace PokedexTL.Infrastructure.DependencyInjection;

public static class RegisterExternalServicesDI
{
    private const string DefaultPokemonApiUrl = "https://pokeapi.co/api/v2/";
    private const string DefaultTransaltionUrl = "https://api.funtranslations.com/translate/";
    public static IServiceCollection RegisterExternalServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IExternalPokemonService, ExternalPokemonService>(
            options =>
                 {
                     options.BaseAddress = new Uri(DefaultPokemonApiUrl);
                 }
            );
        
        serviceCollection.AddHttpClient<IExternalTranslationService, ExternalTranslationService>(
            options =>
                {
                    options.BaseAddress = new Uri(DefaultTransaltionUrl);
                }
            );

        return serviceCollection;
    }
}