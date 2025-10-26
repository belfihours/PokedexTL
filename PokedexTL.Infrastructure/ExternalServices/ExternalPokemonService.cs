using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using PokedexTL.Application.Exceptions;
using PokedexTL.Application.Interfaces;
using PokedexTL.Application.Models;
using PokedexTL.Infrastructure.Configuration;
using PokedexTL.Infrastructure.ExternalModels;

namespace PokedexTL.Infrastructure.ExternalServices;

public class ExternalPokemonService : IExternalPokemonService
{
    private readonly HttpClient _httpClient;
    private readonly ApiPokemonConfiguration _configuration;
    private const string DefaultDescription = "No suitable description found for this pokemon";

    public ExternalPokemonService(
        HttpClient httpClient,
        IOptions<ApiPokemonConfiguration> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration?.Value);
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration.Value;
    }

    public async Task<PokemonDto> GetPokemonAsync(string pokemonName, CancellationToken cancellationToken)
    {
        var pokemonResponse = 
            await _httpClient.GetAsync($"{_configuration.GetPokemonBase()}{pokemonName}", cancellationToken);
        PokemonNotFoundException.ThrowIfNotFound(pokemonName, pokemonResponse.StatusCode);
        pokemonResponse.EnsureSuccessStatusCode();

        var pokemonContent = await pokemonResponse.Content.ReadAsStringAsync(cancellationToken);
        var externalPokemonDto = JsonSerializer.Deserialize<ExternalPokemon>(pokemonContent);
        var species =
            await _httpClient.GetAsync($"{_configuration.GetSpecies()}{externalPokemonDto?.Id}", cancellationToken);
        SpeciesNotFoundException.ThrowIfNotFound(externalPokemonDto!.Id, species.StatusCode);
        species.EnsureSuccessStatusCode();

        var speciesContent = await species.Content.ReadAsStringAsync(cancellationToken);
        var externalSpecies = JsonSerializer.Deserialize<ExternalSpecies>(speciesContent);

        return new PokemonDto
            (
                externalPokemonDto.Name,
                GetDescription(externalSpecies),
                externalSpecies.Habitat.Name,
                externalSpecies.IsLegendary
            );
    }

    private string GetDescription(ExternalSpecies? externalSpecies)
    {
        // To decide whether this Replace has to be kept or leave the original one
        return externalSpecies!.FlavorTextEntries
            .FirstOrDefault(f => f.Language.Name == _configuration.Language)?
            .Text.Replace("\n", " ").Replace("\f", " ").Replace("\r", " ").Replace("\t", " ") 
               ??DefaultDescription;
    }
}