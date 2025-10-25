using PokedexTL.Application.Models;

namespace PokedexTL.Application.Interfaces;

public interface IPokemonService
{
    Task<PokemonDto> GetPokemonAsync(string pokemonName);
    Task<PokemonDto> GetTranslatedPokemonAsync(string pokemonName);
}