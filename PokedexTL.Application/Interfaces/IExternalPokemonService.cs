using PokedexTL.Application.Models;

namespace PokedexTL.Application.Interfaces;

public interface IExternalPokemonService
{
    Task<PokemonDto> GetPokemonAsync(string name);
}