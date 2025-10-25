using PokedexTL.Application.Models;

namespace PokedexTL.Application.Interfaces;

public interface IPokemonService
{
    Task<PokemonDto> GetPokemonAsync(string pokemonName, CancellationToken cancellationToken);
    Task<PokemonDto> GetTranslatedPokemonAsync(string pokemonName, CancellationToken cancellationToken);
}