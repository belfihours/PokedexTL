using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokedexTL.Application.Interfaces;
using PokedexTL.Infrastructure.Configuration;
using PokedexTL.Infrastructure.ExternalServices;

namespace PokedexTL.Infrastructure.DependencyInjection;

public static class RegisterExternalServicesDi
{
    public static IServiceCollection RegisterExternalServices(this IServiceCollection serviceCollection,
        IConfigurationSection configuration)
    {
        var config = configuration.Get<ExternalApisConfiguration>()!;
        serviceCollection.AddHttpClient<IExternalPokemonService, ExternalPokemonService>(
            options =>
                 {
                     options.BaseAddress = new Uri(config.PokeApi.BaseUrl);
                 }
            );
        
        serviceCollection.AddHttpClient<IExternalTranslationService, ExternalTranslationService>(
            options =>
                {
                    options.BaseAddress = new Uri(config.TranslatorApi.BaseUrl);
                }
            );

        return serviceCollection;
    }
}