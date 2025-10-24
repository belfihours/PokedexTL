using PokedexTL.Application.Models;

namespace PokedexTL.Application.Interfaces;

public interface IPokemonService
{
    Task<PokemonDto> GetPokemonAsync(string name);
}